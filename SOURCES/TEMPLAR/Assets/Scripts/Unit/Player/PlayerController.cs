﻿namespace Templar.Unit.Player
{
    using UnityEngine;

    public class PlayerController : UnitController
    {
        [Header("PLAYER")]
        [SerializeField] private PlayerView _playerView = null;
        [SerializeField] private Templar.Camera.CameraController _cameraCtrl = null;
        [SerializeField] private Datas.PlayerControllerDatas _ctrlDatas = null;
        [SerializeField] private LayerMask _rollCollisionMask = 0;

        private System.Collections.IEnumerator _hurtCoroutine;

        private Vector3 _currVel;
        private Vector3 _prevVel;
        private float _refVelX;
        private float _jumpVel;

        public PlayerView PlayerView => _playerView;
        public Templar.Camera.CameraController CameraCtrl => _cameraCtrl;
        public Datas.PlayerControllerDatas CtrlDatas => _ctrlDatas;

        public PlayerInputController InputCtrl { get; private set; }
        public PlayerJumpController JumpCtrl { get; private set; }
        public PlayerRollController RollCtrl { get; private set; }
        public Attack.PlayerAttackController AttackCtrl { get; private set; }

        public float Gravity { get; private set; }

        public bool IsBeingHurt => _hurtCoroutine != null;

        public bool JumpAllowedThisFrame { get; private set; }

        public void Jump()
        {
            _currVel.y = _jumpVel;
        }

        private void OnCollisionDetected(Templar.Physics.CollisionsController.CollisionInfos collisionInfos)
        {
            // Avoid triggering event if there was a collision from the same origin at the previous frame.
            if (CollisionsCtrl.PreviousStates.GetCollisionState(collisionInfos.Origin))
                return;

            switch (collisionInfos.Origin)
            {
                case Templar.Physics.CollisionsController.CollisionOrigin.BELOW:
                {
                    // [BUG]
                    // We need to check if templar was not hurt, else, if he got hurt on top of the skeleton's head,
                    // a collision below is detected afterwards, playing the idle animation.
                    // Solution idea : compute below collision each frame even without y negative velocity ?

                    if (!InputCtrl.CheckInput(PlayerInputController.ButtonCategory.ROLL)
                        && !RollCtrl.IsRolling
                        && !IsBeingHurt)
                    {
                        UnityEngine.Assertions.Assert.IsTrue(_currVel.y < 0f, $"Detected a landing with a positive y velocity ({_currVel.y})!");
                        if (_ctrlDatas.Jump.MinVelForLandImpact > -1 && -_currVel.y > _ctrlDatas.Jump.MinVelForLandImpact)
                            JumpCtrl.TriggerLandImpact(-_currVel.y);
                        else
                            _playerView.PlayIdleAnimation(); // Landing with no speed impact.
                    }

                    // [TMP] GetComponent (2 times), maybe a SkeletonController pooling ?
                    if (collisionInfos.Hit.collider.GetComponent<Enemy.SkeletonController>())
                    {
                        CProLogger.Log(this, "Fell above a skeleton.", gameObject);
                        collisionInfos.Hit.collider.GetComponent<Enemy.SkeletonController>().OnTemplarAbove();
                    }

                    break;
                }

                default:
                    break;
            }
        }

        private void OnUnitHealthChanged(UnitHealthController.UnitHealthChangedEventArgs args)
        {
            float hitDir = args.HitDatas.ComputeHitDir(transform);

            ResetVelocity();

            _playerView.PlayHurtAnimation(hitDir);
            _hurtCoroutine = HurtCoroutine();
            StartCoroutine(_hurtCoroutine);

            CameraCtrl.Shake.SetTrauma(args.HitDatas.AttackDatas.TraumaOnHit);
            if (args.HitDatas.AttackDatas.FreezeFrameDurOnHit > 0f)
                FreezeFrameController.FreezeFrame(0, args.HitDatas.AttackDatas.FreezeFrameDurOnHit);

            _currentRecoil = new Templar.Physics.Recoil(CtrlDatas.HurtRecoilSettings, hitDir);
        }

        private void OnUnitKilled(UnitHealthController.UnitKilledEventArgs args)
        {
            ResetVelocity();

            if (IsBeingHurt)
            {
                StopCoroutine(_hurtCoroutine);
                _hurtCoroutine = null;
            }

            AttackCtrl.CancelAttack();

            CollisionsCtrl.Ground(transform); // [TODO] This doesn't seem to work even if Ground method log looks fine.
            _playerView.PlayDeathAnimation(args.HitDatas.AttackDir);

            CameraCtrl.Shake.SetTrauma(0.5f); // [TMP] Hard coded value.
            _currentRecoil = null;
        }

        [ContextMenu("Compute Jump Physics")]
        private void ComputeJumpPhysics()
        {
            Gravity = -(2f * _ctrlDatas.Jump.JumpHeight) / _ctrlDatas.Jump.JumpApexDurSqr;
            _jumpVel = Mathf.Abs(Gravity) * _ctrlDatas.Jump.JumpApexDur;
        }

        private void ResetVelocity()
        {
            _currVel = Vector3.zero;
        }

        private void TryRoll()
        {
            if (RollCtrl.IsRollingOrInCooldown
                || AttackCtrl.IsAttacking
                || !InputCtrl.CheckInput(PlayerInputController.ButtonCategory.ROLL)
                || !CollisionsCtrl.Below
                || JumpCtrl.IsInLandImpact
                || IsBeingHurt)
                return;

            _currVel = Vector3.zero;
            InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.ROLL);
            RollCtrl.Roll(InputCtrl.Horizontal != 0f ? InputCtrl.Horizontal : CurrDir);
        }

        private void TryAttack()
        {
            if (RollCtrl.IsRolling
                || AttackCtrl.IsAttacking
                || !InputCtrl.CheckInput(PlayerInputController.ButtonCategory.ATTACK)
                || JumpCtrl.IsInLandImpact
                || IsBeingHurt)
                return;

            InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.ATTACK);

            if (CollisionsCtrl.Below)
            {
                AttackCtrl.Attack((attackOverArgs) =>
                {
                    CurrDir = attackOverArgs.Dir;
                    if (AttackCtrl.CurrentAttackDatas.ControlVelocity)
                        ResetVelocity();
                });
            }
            else if (AttackCtrl.CanAttackAirborne)
            {
                AttackCtrl.AttackAirborne((attackOverArgs) =>
                {
                    if (AttackCtrl.CurrentAttackDatas.ControlVelocity)
                        ResetVelocity();
                });
            }
        }

        private void Move()
        {
            if (RollCtrl.IsRolling || AttackCtrl.IsAttacking && AttackCtrl.CurrentAttackDatas.ControlVelocity)
                return;

            if (CollisionsCtrl.Vertical && !JumpCtrl.IsAnticipatingJump)
            {
                _currVel.y = 0f;
                if (CollisionsCtrl.Below)
                {
                    JumpCtrl.ResetJumpsLeft();
                    AttackCtrl.ResetAirborneAttack();
                }
            }

            if (InputCtrl.Horizontal != 0f)
                CurrDir = InputCtrl.CurrentInputDir;

            // Jump.
            if (JumpCtrl.JumpsLeft > 0
                && InputCtrl.CheckInput(PlayerInputController.ButtonCategory.JUMP)
                && !JumpCtrl.IsInLandImpact && !JumpCtrl.IsAnticipatingJump
                && (AttackCtrl.CurrentAttackDatas == null || AttackCtrl.CanChainAttack)
                && !IsBeingHurt)
            {
                JumpAllowedThisFrame = true;
                InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.JUMP);

                if (CollisionsCtrl.Below)
                {
                    if (_ctrlDatas.Jump.JumpAnticipationDur > 0)
                        JumpCtrl.JumpAfterAnticipation();
                    else
                        Jump();
                }
                else
                {
                    CProLogger.Log(this, "Double jump.", gameObject);
                    JumpCtrl.JumpsLeft--;

                    // Airborne jump.
                    if (_ctrlDatas.Jump.AirborneJumpAnticipationDur > 0)
                    {
                        JumpCtrl.JumpAfterAnticipation(true);
                    }
                    else
                    {
                        Jump();
                        _playerView.PlayDoubleJumpAnimation();
                    }
                }
            }

            float targetVelX = _ctrlDatas.RunSpeed;
            if (!IsBeingHurt)
            {
                targetVelX *= InputCtrl.Horizontal;
                if (JumpCtrl.IsAnticipatingJump)
                    targetVelX *= _ctrlDatas.Jump.JumpAnticipationSpeedMult;
                if (JumpCtrl.IsInLandImpact)
                    targetVelX *= JumpCtrl.LandImpactSpeedMult;
            }
            else
            {
                targetVelX = 0f;
            }

            float grav = Gravity * Time.deltaTime;
            if (_currVel.y < 0f)
                grav *= _ctrlDatas.Jump.FallMultiplier;

            _currVel.x = Mathf.SmoothDamp(_currVel.x, targetVelX, ref _refVelX, CollisionsCtrl.Below ? _ctrlDatas.GroundedDamping : _ctrlDatas.Jump.AirborneDamping);
            _currVel.y += grav;
            _currVel.y = Mathf.Max(_currVel.y, -_ctrlDatas.MaxFallVelocity);

            Translate(_currVel);

            // Doing a grounded jump or falling will trigger this condition and remove one jump left. We need to do this after the ComputeCollisions call.
            if (!CollisionsCtrl.Below && CollisionsCtrl.PreviousStates.GetCollisionState(Templar.Physics.CollisionsController.CollisionOrigin.BELOW))
                JumpCtrl.JumpsLeft--;
        }

        private void BackupCurrentState()
        {
            CollisionsCtrl.BackupCurrentState();
            _prevVel = _currVel;
        }

        private void ResetCurrentState()
        {
            InputCtrl.Reset();
            JumpAllowedThisFrame = false;
        }

        private System.Collections.IEnumerator HurtCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(CtrlDatas.HurtDur);
            _hurtCoroutine = null;
            _playerView.PlayIdleAnimation();
        }

        private void Awake()
        {
            InputCtrl = new PlayerInputController(_ctrlDatas.Input, this);
            JumpCtrl = new PlayerJumpController(this);
            RollCtrl = new PlayerRollController(this);
            AttackCtrl = new Attack.PlayerAttackController(this);

            CollisionsCtrl = new Templar.Physics.PlayerCollisionsController(BoxCollider2D, CollisionMask, _rollCollisionMask, this);
            CollisionsCtrl.CollisionDetected += OnCollisionDetected;

            if (HealthCtrl is PlayerHealthController templarHealthCtrl)
            {
                templarHealthCtrl.Init();
                templarHealthCtrl.PlayerCtrl = this;
                templarHealthCtrl.UnitHealthChanged += OnUnitHealthChanged;
                templarHealthCtrl.UnitKilled += OnUnitKilled;
            }

            _playerView.TemplarController = this;

            ComputeJumpPhysics();

            if (_ctrlDatas.GroundOnAwake)
                CollisionsCtrl.Ground(transform);

            CurrDir = _playerView.GetSpriteRendererFlipX() ? -1f : 1f;
        }

        private void Update()
        {
            BackupCurrentState();
            ResetCurrentState();

            InputCtrl.Update();

            if (IsDead)
                return;

            TryRoll();
            TryAttack();
            Move();

            if (_currentRecoil != null)
            {
                Translate(new Vector3(_currentRecoil.Dir * _currentRecoil.Force, 0f));
                _currentRecoil.Update();
                if (_currentRecoil.IsComplete)
                    _currentRecoil = null;
            }

            CollisionsCtrl.TriggerDetectedCollisionsEvents();
            _playerView.UpdateView(flip: CurrDir != 1f, _currVel, _prevVel);
        }

        private void OnDestroy()
        {
            CollisionsCtrl.CollisionDetected -= OnCollisionDetected;

            if (HealthCtrl is PlayerHealthController templarHealthCtrl)
            {
                templarHealthCtrl.UnitHealthChanged -= OnUnitHealthChanged;
                templarHealthCtrl.UnitKilled -= OnUnitKilled;
            }
        }
    }
}