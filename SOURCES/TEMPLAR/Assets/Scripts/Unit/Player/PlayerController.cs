namespace Templar.Unit.Player
{
    using RSLib.Extensions;
    using UnityEngine;

    public class PlayerController : UnitController
    {
        [Header("PLAYER")]
        [SerializeField] private PlayerView _playerView = null;
        [SerializeField] private Templar.Camera.CameraController _cameraCtrl = null; // Player should not reference the camera.
        [SerializeField] private Datas.Unit.Player.PlayerControllerDatas _ctrlDatas = null;
        [SerializeField] private Interaction.Interacter _interacter = null;
        [SerializeField] private LayerMask _rollCollisionMask = 0;
        [SerializeField] private int _baseHealth = 100;

        private bool _inputsAllowed;

        private System.Collections.IEnumerator _hurtCoroutine;
        private System.Collections.IEnumerator _healCoroutine;

        private Vector3 _currVel;
        private Vector3 _prevVel;
        private float _refVelX;
        private float _jumpVel;

        public bool Initialized { get; private set; }

        public PlayerView PlayerView => _playerView;
        public Templar.Camera.CameraController CameraCtrl => _cameraCtrl;
        public Datas.Unit.Player.PlayerControllerDatas CtrlDatas => _ctrlDatas;

        public PlayerHealthController PlayerHealthCtrl => HealthCtrl as PlayerHealthController;

        public PlayerInputController InputCtrl { get; private set; }
        public PlayerJumpController JumpCtrl { get; private set; }
        public PlayerRollController RollCtrl { get; private set; }
        public Attack.PlayerAttackController AttackCtrl { get; private set; }

        public float Gravity { get; private set; }

        public bool IsBeingHurt => _hurtCoroutine != null;
        public bool IsHealing => _healCoroutine != null;

        public bool JumpAllowedThisFrame { get; private set; }

        public void Init(Interaction.CheckpointController checkpoint = null)
        {
            InputCtrl = new PlayerInputController(CtrlDatas.Input, this);
            JumpCtrl = new PlayerJumpController(this);
            RollCtrl = new PlayerRollController(this);
            AttackCtrl = new Attack.PlayerAttackController(this);

            CollisionsCtrl = new Templar.Physics.PlayerCollisionsController(BoxCollider2D, CollisionMask, _rollCollisionMask, this);
            CollisionsCtrl.CollisionDetected += OnCollisionDetected;

            if (HealthCtrl is PlayerHealthController templarHealthCtrl)
            {
                templarHealthCtrl.PlayerCtrl = this;
                templarHealthCtrl.Init(_baseHealth);
                templarHealthCtrl.UnitHealthChanged += OnUnitHealthChanged;
                templarHealthCtrl.UnitKilled += OnUnitKilled;
            }

            PlayerView.TemplarController = this;

            ComputeJumpPhysics();

            if (checkpoint != null)
                transform.position = checkpoint.RespawnPos.AddY(Templar.Physics.RaycastsController.SKIN_WIDTH * 10f);

            if (CtrlDatas.GroundOnAwake)
                CollisionsCtrl.Ground(transform);

            CurrDir = PlayerView.GetSpriteRendererFlipX() ? -1f : 1f;

            Initialized = true;
        }

        public void AllowInputs(bool state)
        {
            _inputsAllowed = state;
        }

        public void Jump()
        {
            _currVel.y = _jumpVel;
        }

        protected override void OnCollisionDetected(Templar.Physics.CollisionsController.CollisionInfos collisionInfos)
        {
            base.OnCollisionDetected(collisionInfos);

            if (IsDead)
                return;

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
                        if (CtrlDatas.Jump.MinVelForLandImpact > -1 && -_currVel.y > CtrlDatas.Jump.MinVelForLandImpact)
                            JumpCtrl.TriggerLandImpact(-_currVel.y);
                        else
                            PlayerView.PlayIdleAnimation(); // Landing with no speed impact.
                    }

                    break;
                }

                default:
                    break;
            }
        }

        private void OnUnitHealthChanged(UnitHealthController.UnitHealthChangedEventArgs args)
        {
            // [TODO] Think of a better way to handle all possible damage sources (fall, debug, hits, poison, traps).

            if (!args.IsLoss)
                return;

            if (IsHealing)
            {
                StopCoroutine(_healCoroutine);
                _healCoroutine = null;
            }

            float hitDir = args.HitDatas.ComputeHitDir(transform);

            ResetVelocity();

            PlayerView.PlayHurtAnimation(hitDir);
            _hurtCoroutine = HurtCoroutine();
            StartCoroutine(_hurtCoroutine);

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
            RollCtrl.Interrupt();

            CollisionsCtrl.Ground(transform); // [TODO] This doesn't seem to work even if Ground method log looks fine.
            PlayerView.PlayDeathAnimation(args.HitDatas?.AttackDir ?? CurrDir);

            CameraCtrl.Shake.SetTrauma(0.5f); // [TMP] Hard coded value.
            Manager.RampFadeManager.Fade(CameraCtrl.GrayscaleRamp, "OutBase", (1.5f, 1f), RSLib.SceneReloader.ReloadScene);
            _currentRecoil = null;
        }

        [ContextMenu("Compute Jump Physics")]
        private void ComputeJumpPhysics()
        {
            Gravity = -(2f * CtrlDatas.Jump.JumpHeight) / CtrlDatas.Jump.JumpApexDurSqr;
            _jumpVel = Mathf.Abs(Gravity) * CtrlDatas.Jump.JumpApexDur;
        }

        private void ResetVelocity()
        {
            _currVel = Vector3.zero;
        }

        private void TryRoll()
        {
            if (RollCtrl.IsRollingOrInCooldown
                || AttackCtrl.IsAttacking
                || !CollisionsCtrl.Below
                || JumpCtrl.IsInLandImpact
                || IsBeingHurt
                || IsHealing
                || !InputCtrl.CheckInput(PlayerInputController.ButtonCategory.ROLL))
                return;

            InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.ROLL);
            _currVel = Vector3.zero;

            RollCtrl.Roll(
                InputCtrl.Horizontal != 0f ? Mathf.Sign(InputCtrl.Horizontal) : CurrDir,
                (args) => _currVel = args.Vel);
        }

        private void TryAttack()
        {
            if (RollCtrl.IsRolling
                || AttackCtrl.IsAttacking
                || JumpCtrl.IsInLandImpact
                || IsBeingHurt
                || IsHealing
                || !InputCtrl.CheckInput(PlayerInputController.ButtonCategory.ATTACK))
                return;

            InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.ATTACK);

            if (CollisionsCtrl.Below)
            {
                AttackCtrl.Attack((attackOverArgs) =>
                {
                    CurrDir = attackOverArgs.Dir;
                    if (AttackCtrl.CurrAttackDatas.ControlVelocity)
                        ResetVelocity();
                });
            }
            else if (AttackCtrl.CanAttackAirborne)
            {
                AttackCtrl.AttackAirborne((attackOverArgs) =>
                {
                    if (((Datas.Attack.PlayerAttackDatas)attackOverArgs.AttackDatas).ControlVelocity)
                        ResetVelocity();
                });
            }
        }

        private void TryInteract()
        {
            // [TMP] Maybe we'll want conditions to disallow interaction while attacking or something but
            // this will cause issues since interaction feedback is triggered by physic collisions.
            // If the player attacks, we need to remove feedback on potential interactable, but there won't be
            // a collision afterward to show it again. Callback system ?

            if (IsHealing || !InputCtrl.CheckInput(PlayerInputController.ButtonCategory.INTERACT))
                return;

            _interacter.TryInteract();
            InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.INTERACT);
        }

        private void TryHeal()
        {
            if (RollCtrl.IsRolling
                || AttackCtrl.IsAttacking
                || JumpCtrl.IsInLandImpact
                || IsBeingHurt
                || IsHealing
                || HealthCtrl.HealthSystem.IsFull
                || PlayerHealthCtrl.HealCellsLeft == 0
                || !InputCtrl.CheckInput(PlayerInputController.ButtonCategory.HEAL))
                return;

            InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.HEAL);

            PlayerView.PlayHealAnimation(() =>
            {
                UnityEngine.Assertions.Assert.IsTrue(PlayerHealthCtrl.HealCellsLeft > 0, "Healing has been allowed while there are no cells left.");
                PlayerHealthCtrl.HealCellsLeft--;
                HealthCtrl.HealthSystem.Heal(50); // [TODO] Hardcoded values.

                _cameraCtrl.Shake.AddTrauma(0.25f); // [TODO] Hardcoded values.
                if (CtrlDatas.HealRecoilSettings != null)
                    _currentRecoil = new Templar.Physics.Recoil(CtrlDatas.HealRecoilSettings, -CurrDir);
            });

            _healCoroutine = HealCoroutine();
            StartCoroutine(_healCoroutine);
        }

        private void Move()
        {
            if (RollCtrl.IsRolling || AttackCtrl.IsAttacking && AttackCtrl.CurrAttackDatas.ControlVelocity)
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
                CurrDir = InputCtrl.CurrentHorizontalDir;

            // Jump.
            if (JumpCtrl.JumpsLeft > 0
                && InputCtrl.CheckInput(PlayerInputController.ButtonCategory.JUMP)
                && !JumpCtrl.IsInLandImpact && !JumpCtrl.IsAnticipatingJump
                && (AttackCtrl.CurrAttackDatas == null || AttackCtrl.CanChainAttack)
                && !IsBeingHurt
                && !IsHealing)
            {
                JumpAllowedThisFrame = true;
                InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.JUMP);

                if (CollisionsCtrl.Below)
                {
                    if (CtrlDatas.Jump.JumpAnticipationDur > 0)
                        JumpCtrl.JumpAfterAnticipation();
                    else
                        Jump();
                }
                else
                {
                    CProLogger.Log(this, "Double jump.", gameObject);
                    JumpCtrl.JumpsLeft--;

                    // Airborne jump.
                    if (CtrlDatas.Jump.AirborneJumpAnticipationDur > 0)
                    {
                        JumpCtrl.JumpAfterAnticipation(true);
                    }
                    else
                    {
                        Jump();
                        PlayerView.PlayDoubleJumpAnimation();
                    }
                }
            }

            float targetVelX = CtrlDatas.RunSpeed;
            if (!IsBeingHurt && !IsHealing)
            {
                targetVelX *= InputCtrl.Horizontal;
                if (JumpCtrl.IsAnticipatingJump)
                    targetVelX *= CtrlDatas.Jump.JumpAnticipationSpeedMult;
                if (JumpCtrl.IsInLandImpact)
                    targetVelX *= JumpCtrl.LandImpactSpeedMult;
            }
            else
            {
                targetVelX = 0f;
            }

            float grav = Gravity * Time.deltaTime;
            if (_currVel.y < 0f)
                grav *= CtrlDatas.Jump.FallMultiplier;

            _currVel.x = Mathf.SmoothDamp(_currVel.x, targetVelX, ref _refVelX, CollisionsCtrl.Below ? CtrlDatas.GroundedDamping : CtrlDatas.Jump.AirborneDamping);
            _currVel.y += grav;
            _currVel.y = Mathf.Max(_currVel.y, -CtrlDatas.MaxFallVelocity);

            _currVel += GetCurrentRecoil(); // Done here because events management seems to have a better behaviour when all movement is done in one place.

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
            PlayerView.PlayIdleAnimation();
        }

        private System.Collections.IEnumerator HealCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(CtrlDatas.HealDur);
            _healCoroutine = null;
            PlayerView.PlayIdleAnimation();
        }

        protected override void Update()
        {
            if (!Initialized)
                return;

            base.Update();

            BackupCurrentState();
            ResetCurrentState();

            if (_inputsAllowed)
                InputCtrl.Update();

            if (IsDead)
                return;

            TryRoll();
            TryAttack();
            TryInteract();
            TryHeal();

            Move();
            //ApplyCurrentRecoil();

            CollisionsCtrl.TriggerDetectedCollisionsEvents();
            PlayerView.UpdateView(flip: CurrDir != 1f, _currVel, _prevVel);
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