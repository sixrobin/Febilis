namespace Templar.Unit.Player
{
    using RSLib.Extensions;
    using SceneLoadingDatasStorage;
    using UnityEngine;

    public class PlayerController : UnitController, ICheckpointListener, Interaction.Dialogue.ISpeaker, ISceneLoadingDatasOwner<SceneLoadDatasPlayer>
    {
        private const string PLAYER_SPEAKER_ID = "Templar";
        private const int EFFECTOR_DOWN_FRAME_DUR = 10;

        [Header("PLAYER")]
        [SerializeField] private PlayerView _playerView = null;
        [SerializeField] private Datas.Unit.Player.PlayerControllerDatas _ctrlDatas = null;
        [SerializeField] private Interaction.Interacter _interacter = null;
        [SerializeField] private LayerMask _rollCollisionMask = 0;
        [SerializeField] private int _baseHealth = 100;

        private bool _inputsAllowed;

        private System.Collections.IEnumerator _hurtCoroutine;
        private System.Collections.IEnumerator _healCoroutine;
        private System.Collections.IEnumerator _effectorDownCoroutine;

        private Vector3 _currVel;
        private Vector3 _prevVel;
        private float _refVelX;

        private float _debugSpeedMult = 1f;
        private float _debugJumpMult = 1f;

        public bool Initialized { get; private set; }

        public PlayerView PlayerView => _playerView;
        public override UnitView UnitView => _playerView;

        public Datas.Unit.Player.PlayerControllerDatas CtrlDatas => _ctrlDatas;

        public PlayerHealthController PlayerHealthCtrl => HealthCtrl as PlayerHealthController;

        public PlayerInputController InputCtrl { get; private set; }
        public PlayerJumpController JumpCtrl { get; private set; }
        public PlayerRollController RollCtrl { get; private set; }
        public Attack.PlayerAttackController AttackCtrl { get; private set; }

        public bool WasFallingLastFrame => _currVel.y < 0f && !CollisionsCtrl.PreviousStates.GetCollisionState(Templar.Physics.CollisionsController.CollisionOrigin.BELOW);

        public bool IsBeingHurt => _hurtCoroutine != null;
        public bool IsHealing => _healCoroutine != null;
        public bool EffectorDown => _effectorDownCoroutine != null;

        public bool IsDialoguing { get; set; }
        public string SpeakerId => PLAYER_SPEAKER_ID;
        public Vector3 SpeakerPos => transform.position;

        public SceneLoadDatasPlayer SaveDatasBeforeSceneLoading()
        {
            return new SceneLoadDatasPlayer()
            {
                CurrentHealth = HealthCtrl.HealthSystem.CurrentHealth
            };
        }

        public void LoadDatasAfterSceneLoading(SceneLoadDatasPlayer datas)
        {
            PlayerHealthCtrl.SetHealth(datas.CurrentHealth, false);
        }

        void ICheckpointListener.OnCheckpointInteracted(Interaction.Checkpoint.CheckpointController checkpointCtrl)
        {
            HealthCtrl.HealFull();

            // [TMP] Constant value of 2 max potions.
            int currentPotionsQuantity = Manager.GameManager.InventoryCtrl.GetItemQuantity(Item.InventoryController.ITEM_ID_POTION);
            if (currentPotionsQuantity < 2)
                Manager.GameManager.InventoryCtrl.AddItem(Item.InventoryController.ITEM_ID_POTION, 2 - currentPotionsQuantity, showPickupNotification: false);

            AllowInputs(true);
        }

        void Interaction.Dialogue.ISpeaker.OnSentenceStart()
        {
            PlayerView.PlayDialogueTalkAnimation();
        }

        void Interaction.Dialogue.ISpeaker.OnSentenceEnd()
        {
            PlayerView.PlayDialogueIdleAnimation();
        }

        public void Init(Interaction.Checkpoint.CheckpointController checkpoint = null)
        {
            if (Initialized)
                return;

            InputCtrl = new PlayerInputController(CtrlDatas.Input, this);
            JumpCtrl = new PlayerJumpController(this);
            RollCtrl = new PlayerRollController(this);
            AttackCtrl = new Attack.PlayerAttackController(this);

            CollisionsCtrl = new Templar.Physics.PlayerCollisionsController(BoxCollider2D, CollisionMask, _rollCollisionMask, this);
            CollisionsCtrl.CollisionDetected += OnCollisionDetected;

            PlayerHealthCtrl.PlayerCtrl = this;
            PlayerHealthCtrl.Init(_baseHealth, OnUnitHealthChanged, OnUnitKilled);

            JumpCtrl.ComputeJumpPhysics();

            if (checkpoint != null)
                transform.position = checkpoint.RespawnPos.AddY(Templar.Physics.RaycastsController.SKIN_WIDTH * 10f);

            if (CtrlDatas.GroundOnAwake)
                CollisionsCtrl.Ground(transform);

            PlayerView.PlayerCtrl = this;
            CurrDir = PlayerView.GetSpriteRendererFlipX() ? -1f : 1f;

            CtrlDatas.ValuesValidated += OnDatasValuesChanged;

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<float, float>("PositionAdd", $"Adds a vector to the player position.", (x, y) => { transform.position += new Vector3(x, y); }));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<float, float>("PositionSet", $"Sets the player position.", (x, y) => { transform.position = new Vector3(x, y); }));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<float>("MultiplySpeed", $"Multiplies player speed.", (x) =>  _debugSpeedMult = x ));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<float>("MultiplyJump", $"Multiplies player jump.", (x) =>  _debugJumpMult = x ));

            Initialized = true;
        }

        public void AllowInputs(bool state)
        {
            _inputsAllowed = state;
        }

        public void JumpWithMaxVelocity()
        {
            _currVel.y = JumpCtrl.JumpVelMax * _debugJumpMult;
        }

        public void JumpWithVariousVelocity()
        {
            _currVel.y = (InputCtrl.CheckJumpInput() ? JumpCtrl.JumpVelMax : JumpCtrl.JumpVelMin) * _debugJumpMult;
        }

        public void ResetVelocity()
        {
            _currVel = Vector3.zero;
        }

        public override void Translate(Vector3 vel, bool triggerEvents = true, bool checkEdge = false, bool effectorDown = false, bool standingOnPlatform = false)
        {
            // We don't want to use the base method because it computes both direction and then translates, which can result
            // in glitchy corners collisions. This is fine for enemies, but for the player we want to check any direction first, translate
            // if needed, refresh the raycast origins, then check the other direction, and translate if needed.

            CollisionsCtrl.ComputeRaycastOrigins();
            CollisionsCtrl.CurrentStates.Reset();

            if (standingOnPlatform)
                CollisionsCtrl.CurrentStates.SetCollision(Templar.Physics.CollisionsController.CollisionOrigin.BELOW);

            vel *= Time.deltaTime;

            if (vel.x != 0f)
            {
                CollisionsCtrl.ComputeHorizontalCollisions(ref vel, triggerEvents, checkEdge);
                transform.Translate(new Vector3(vel.x, 0f));
            }

            if (vel.y != 0f)
            {
                CollisionsCtrl.ComputeRaycastOrigins();
                CollisionsCtrl.ComputeVerticalCollisions(ref vel, effectorDown, triggerEvents);
                transform.Translate(new Vector3(0f, vel.y));
            }
        }

        public void TriggerHeal()
        {
            InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.HEAL);
            StartCoroutine(_healCoroutine = HealCoroutine());
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

            if (args.HitDatas == null)
            {
                CProLogger.LogWarning(this, "Player health changed but HitDatas is null. Allowing it but make sure it's for debugging purpose.");
                return;
            }

            if (IsHealing)
            {
                StopCoroutine(_healCoroutine);
                _healCoroutine = null;
            }

            if (AttackCtrl.IsAttacking && AttackCtrl.CurrAttackDatas.Unstoppable)
                return;

            AttackCtrl.CancelAttack();
            ResetVelocity();

            float hitDir = args.HitDatas.ComputeHitDir(transform);
            _currentRecoil = new Templar.Physics.Recoil(hitDir, args.HitDatas.AttackDatas.RecoilDatas);
            StartCoroutine(_hurtCoroutine = HurtCoroutine(hitDir));
        }

        private void OnUnitKilled(UnitHealthController.UnitKilledEventArgs args)
        {
            ResetVelocity();

            Manager.SaveManager.Save(); // [TMP] Save picked up items upon dying.

            if (IsBeingHurt)
            {
                StopCoroutine(_hurtCoroutine);
                _hurtCoroutine = null;
            }

            AttackCtrl.CancelAttack();
            RollCtrl.Interrupt();

            CollisionsCtrl.Ground(transform); // [TODO] This doesn't seem to work even if Ground method log looks fine.
            PlayerView.PlayDeathAnimation(args.HitDatas?.AttackDir ?? CurrDir);

            _currentRecoil = null;

            Manager.GameManager.CameraCtrl.GetShake(Templar.Camera.CameraShake.ID_BIG).SetTrauma(0.5f); // [TMP] Hard coded value.
            Manager.RampFadeManager.Fade(Manager.GameManager.CameraCtrl.GrayscaleRamp, "InBase", (1.5f, 1f), (fadeIn) => RSLib.SceneReloader.ReloadScene());

            StartDeadFadeCoroutine();
        }

        private void TryRoll()
        {
            if (!RollCtrl.CanRoll())
                return;

            InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.ROLL);
            _currVel = Vector3.zero;

            RollCtrl.Roll(
                InputCtrl.Horizontal != 0f ? Mathf.Sign(InputCtrl.Horizontal) : CurrDir,
                (args) =>
                {
                    _currVel = args.Vel;
                    if (!CollisionsCtrl.Below)
                        JumpCtrl.JumpsLeft--;
                });
        }

        private void TryAttack()
        {
            if (!AttackCtrl.CanAttack())
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

            InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.INTERACT);
            _interacter.TryInteract();
        }

        private void TryHeal()
        {
            if (PlayerHealthCtrl.CanHeal())
                TriggerHeal();
        }

        private void Move()
        {
            if (RollCtrl.IsRolling || AttackCtrl.IsAttacking && AttackCtrl.CurrAttackDatas.ControlVelocity || IsHealing)
                return;

            if (CollisionsCtrl.Vertical && !JumpCtrl.IsAnticipatingJump)
            {
                if (CollisionsCtrl.Below)
                {
                    JumpCtrl.ResetJumpsLeft();
                    AttackCtrl.ResetAirborneAttack();
                }

                if (InputCtrl.CheckInput(PlayerInputController.ButtonCategory.JUMP) && InputCtrl.Vertical == -1f)
                    StartCoroutine(_effectorDownCoroutine = EffectorDownCoroutine());

                if (!EffectorDown)
                    _currVel.y = 0f;
            }

            if (InputCtrl.Horizontal != 0f && !IsHealing)
                CurrDir = InputCtrl.CurrentHorizontalDir;

            // Jump.
            if (JumpCtrl.CanJump() && (!EffectorDown || !CollisionsCtrl.AboveEffector))
            {
                JumpCtrl.JumpAllowedThisFrame = true;
                InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.JUMP);

                if (CollisionsCtrl.Below)
                {
                    if (CtrlDatas.Jump.JumpAnticipationDur > 0)
                        JumpCtrl.JumpAfterAnticipation();
                    else
                        JumpWithMaxVelocity();
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
                        JumpWithMaxVelocity();
                        PlayerView.PlayDoubleJumpAnimation();
                    }
                }
            }

            // Jump lower if jump input is released.
            if (!CollisionsCtrl.Below && !JumpCtrl.IsAnticipatingJump && !InputCtrl.CheckJumpInput() && _currVel.y > JumpCtrl.JumpVelMin)
                _currVel.y = JumpCtrl.JumpVelMin;

            if (EffectorDown && CollisionsCtrl.AboveEffector)
                StartCoroutine(ResetJumpInputAfterDownEffector());

            float targetVelX = CtrlDatas.RunSpeed * _debugSpeedMult;
            if (!IsBeingHurt && !IsHealing)
            {
                targetVelX *= InputCtrl.Horizontal;

                // Clamp value to a minimum, to avoid players settings their dead zone to a low value being too slow.
                if (InputCtrl.Horizontal > 0f)
                    targetVelX = Mathf.Max(targetVelX, CtrlDatas.MinRunSpeed);
                else if (InputCtrl.Horizontal < 0f)
                    targetVelX = Mathf.Min(targetVelX, -CtrlDatas.MinRunSpeed);

                if (JumpCtrl.IsAnticipatingJump)
                    targetVelX *= CtrlDatas.Jump.JumpAnticipationSpeedMult;
                if (JumpCtrl.IsInLandImpact)
                    targetVelX *= JumpCtrl.LandImpactSpeedMult;
            }
            else
            {
                targetVelX = 0f;
            }

            float grav = JumpCtrl.Gravity * Time.deltaTime;
            if (_currVel.y < 0f)
                grav *= CtrlDatas.Jump.FallMultiplier;

            _currVel.x = Mathf.SmoothDamp(_currVel.x, targetVelX, ref _refVelX, CollisionsCtrl.Below ? CtrlDatas.GroundedDamping : CtrlDatas.Jump.AirborneDamping);
            _currVel.y += grav;
            _currVel.y = Mathf.Max(_currVel.y, -CtrlDatas.MaxFallVelocity);

            // Doing this here makes events being well triggered but causes the tennis ball bug.
            //_currVel += GetCurrentRecoil();

            Translate(_currVel, checkEdge: false, effectorDown: EffectorDown);

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
            JumpCtrl.JumpAllowedThisFrame = false;
        }

        /// <summary>Moves the player to a given position before doing an interaction.</summary>
        /// <param name="interactablePos">Source interaction position, that the player will look at.</param>
        /// <param name="playerInteractionPivot">Position the player will reach before looking at interactable source, and interacting with it. Can be null.</param>
        public System.Collections.IEnumerator GoToInteractionPosition(Vector3 interactablePos, Transform playerInteractionPivot, float timeout = 0f)
        {
            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame; // Without this wait, player will play its idle animation back due to Update().

            AttackCtrl.CancelAttack();
            RollCtrl.Interrupt();

            if (playerInteractionPivot == null)
            {
                LookAt(interactablePos);
                yield break;
            }

            CurrDir = Mathf.Sign(playerInteractionPivot.position.x - transform.position.x);
            PlayerView.PlayRunAnimation(CurrDir);

            float timeoutTimer = 0f;
            while (Mathf.Abs(transform.position.x - playerInteractionPivot.position.x) > CtrlDatas.RunSpeed * Time.deltaTime + 0.05f)
            {
                // [TMP] Not sure if actually TMP, but maybe we should think of a better way to do this because it currently
                // is just a copy/paste of the Move() method.

                if (timeout > 0f)
                {
                    timeoutTimer += Time.deltaTime;
                    if (timeoutTimer >= timeout)
                    {
                        CProLogger.LogError(this, $"{nameof(GoToInteractionPosition)} coroutine timed out.", gameObject);
                        break;
                    }
                }

                if (CollisionsCtrl.Below)
                    _currVel.y = 0f;

                float grav = JumpCtrl.Gravity * Time.deltaTime;
                if (_currVel.y < 0f)
                    grav *= CtrlDatas.Jump.FallMultiplier;

                _currVel.x = CtrlDatas.RunSpeed * CurrDir;
                _currVel.y += grav;

                Translate(_currVel);
                yield return null;
            }

            PlayerView.StopRunAnimation();
            if (timeoutTimer < timeout) // Timeout is used when blocked on a wall so we should not teleport the player inside the wall.
                transform.SetPositionX(playerInteractionPivot.position.x);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(0.5f);

            LookAt(interactablePos);
        }

        public System.Collections.IEnumerator MoveToDirection(float dir, float dur)
        {
            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame; // Without this wait, player will play its idle animation back due to Update().
            yield return new WaitUntil(() => !AttackCtrl.IsAttacking && !RollCtrl.IsRolling && CollisionsCtrl.Below);

            CurrDir = dir;
            PlayerView.PlayIdleAnimation();

            float timer = 0f;
            while (timer < dur)
            {
                if (CollisionsCtrl.Below)
                {
                    PlayerView.PlayRunAnimation(CurrDir);
                    _currVel.y = 0f;
                }

                float grav = JumpCtrl.Gravity * Time.deltaTime;
                if (_currVel.y < 0f)
                    grav *= CtrlDatas.Jump.FallMultiplier;

                _currVel.x = CtrlDatas.RunSpeed * CurrDir;
                _currVel.y += grav;

                Translate(_currVel);

                timer += Time.deltaTime;
                yield return null;
            }

            PlayerView.StopRunAnimation();
        }
        
        public System.Collections.IEnumerator KeepUpwardMovement(float dur)
        {
            float yVel = _currVel.y;

            float timer = 0f;
            while (timer < dur)
            {
                _currVel = new Vector3(0f, yVel);
                Translate(_currVel);

                timer += Time.deltaTime;
                yield return null;
            }
        }

        private System.Collections.IEnumerator HurtCoroutine(float dir)
        {
            PlayerView.PlayHurtAnimation(dir);
            yield return RSLib.Yield.SharedYields.WaitForSeconds(CtrlDatas.HurtDur);
            _hurtCoroutine = null;
            PlayerView.PlayIdleAnimation();
        }

        private System.Collections.IEnumerator HealCoroutine()
        {
            PlayerView.PlayHealAnticipationAnimation();
            yield return RSLib.Yield.SharedYields.WaitForSeconds(CtrlDatas.PreHealDelay);

            if (!PlayerHealthCtrl.DebugMode)
            {
                UnityEngine.Assertions.Assert.IsTrue(PlayerHealthCtrl.HealsLeft > 0, "Healing has been allowed while there are no cells left.");
                PlayerHealthCtrl.HealsLeft--;
            }

            HealthCtrl.HealthSystem.Heal(PlayerHealthCtrl.HealAmount);

            Manager.GameManager.CameraCtrl.GetShake(Templar.Camera.CameraShake.ID_SMALL).AddTrauma(0.25f, 0.4f); // [TODO] Hardcoded values.
            _currentRecoil = new Templar.Physics.Recoil(-CurrDir, new Datas.Attack.RecoilDatas(1f, 0.1f)); // [TODO] Hardcoded values.

            PlayerView.PlayHealAnimation();
            yield return RSLib.Yield.SharedYields.WaitForSeconds(CtrlDatas.PostHealDelay);

            _healCoroutine = null;
            PlayerView.PlayIdleAnimation();
        }

        private System.Collections.IEnumerator ResetJumpInputAfterDownEffector()
        {
            // Used to avoid player to jump after falling down from an effector too close to the ground.
            yield return RSLib.Yield.SharedYields.WaitForSeconds(0.02f);
            InputCtrl.ResetDelayedInput(PlayerInputController.ButtonCategory.JUMP);
        }

        private System.Collections.IEnumerator EffectorDownCoroutine()
        {
            // Allow player to fall down even if on a downward moving platform.
            // Coroutine duration should be based on platform velocity, but this should be okay for most of the cases.
            yield return RSLib.Yield.SharedYields.WaitForFrames(EFFECTOR_DOWN_FRAME_DUR);
            _effectorDownCoroutine = null;
        }

        protected override void Update()
        {
            if (!Initialized)
                return;

            base.Update();

            BackupCurrentState();
            ResetCurrentState();

            // Can't use the _inputsAllowed field for dialogue while this Update() is updating the view.
            if (IsDialoguing)
                return;

            if (_inputsAllowed
                && !Tools.CheckpointTeleporter.IsOpen
                && !Manager.BoardsTransitionManager.IsInBoardTransition
                && !Manager.OptionsManager.AnyPanelOpenOrClosedThisFrame()
                && (!Manager.GameManager.InventoryView?.Displayed ?? true))
                InputCtrl.Update();

            if (IsDead)
                return;

            TryRoll();
            TryAttack();
            TryInteract();
            TryHeal();

            Move();
            ApplyCurrentRecoil();

            CollisionsCtrl.TriggerDetectedCollisionsEvents();

            PlayerView.UpdateView(flip: CurrDir != 1f, _currVel, _prevVel);
        }

        private void OnDestroy()
        {
            CollisionsCtrl.CollisionDetected -= OnCollisionDetected;
            PlayerHealthCtrl.UnitHealthChanged -= OnUnitHealthChanged;
            PlayerHealthCtrl.UnitKilled -= OnUnitKilled;
            CtrlDatas.ValuesValidated -= OnDatasValuesChanged;
        }

        private void OnDatasValuesChanged()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                if (InputCtrl.InputDatas != CtrlDatas.Input)
                    InputCtrl = new PlayerInputController(CtrlDatas.Input, this);
            }
#endif
        }
    }
}