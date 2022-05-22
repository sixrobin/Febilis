namespace Templar.Unit.Player
{
    using RSLib.Extensions;
    using UnityEngine;

    public class PlayerView : UnitView
    {
        private const float DEAD_FADE_DELAY = 1.2f;

        private const string ATTACK_ANM_OVERRIDE_ID = "Attack";
        private const string ATTACK_CLIP_NAME_FORMAT = "Anm_Templar_{0}";

        private const string IS_RUNNING = "IsRunning";
        private const string IDLE_BREAK = "IdleBreak";
        private const string IDLE_SLEEPING = "IdleSleeping";
        private const string FALL = "Fall";
        private const string JUMP = "Jump";
        private const string LAND = "Land";
        private const string ROLL = "Roll";
        private const string HEAL_ANTICIPATION = "HealAnticipation";
        private const string HEAL = "Heal";
        private const string DIALOGUE_IDLE = "DialogueIdle";
        private const string DIALOGUE_TALK = "DialogueTalk";
        private const string LOOK_UP = "LookUp";
        private const string LOOK_DOWN = "LookDown";
        private const string TRANSITION_IN = "TransitionIn";
        private const string TRANSITION_OUT = "TransitionOut";
        private const string MULT_ROLL = "Mult_Roll";
        
        [Header("MOTION")]
        [SerializeField] private GameObject _jumpPuffPrefab = null;
        [SerializeField] private GameObject _doubleJumpPuffPrefab = null;
        [SerializeField] private GameObject _landPuffPrefab = null;
        [SerializeField] private GameObject _rollPuffPrefab = null;
        [SerializeField, Min(0f)] private float _landPuffMinVel = 5f;

        [Header("FIGHT")]
        [SerializeField] private GameObject _hitPrefab = null;
        [SerializeField] private Transform _hitVFXPivot = null;
        [SerializeField] private GameObject _attackPuffPrefab = null;
        [SerializeField] private GameObject[] _hurtPrefabs = null;

        [Header("IDLE BREAK")]
        [SerializeField, Min(0f)] private float _idleBreakSecInterval = 15f;
        [SerializeField, Min(0)] private int _breaksBeforeSleep = 2;
        [SerializeField] private SleepFeedback _sleepFeedback = null;
        
        [Header("HEAL - PLAYER")]
        [SerializeField, Min(0f)] private float _healTrauma = 0.15f;
        
        [Header("DEATH - PLAYER")]
        [SerializeField, Min(0f)] private float _deathStencilDuration = 0.2f;
        
        [Header("AUDIO - PLAYER")]
        [SerializeField] private RSLib.Audio.ClipProvider _landClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _softLandClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _rollClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _jumpClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _doubleJumpClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _healClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _cantHealClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _killTriggerHitClipProvider = null;
        
        [Header("DEBUG")]
        [SerializeField] private bool _logAnimationsPlays = false;

        private int _idleStateHash;
        private int _idleBreakStateHash;
        private int _idleSleepingStateHash;
        private int _fallStateHash;

        private int _currStateHash;
        private int _previousStateHash;

        private float _idleBreakTimer;
        private int _idleBreaksCounter;

        public delegate void SleepAnimationEventHandler();

        public event SleepAnimationEventHandler SleepAnimationBegan;
        public event SleepAnimationEventHandler SleepAnimationOver;

        public PlayerController PlayerCtrl { get; set; }

        public override float DeadFadeDelay => DEAD_FADE_DELAY;

        public void UpdateView(bool flip, Vector3 currVel, Vector3 prevVel)
        {
            _animator.SetBool(IS_RUNNING, !PlayerCtrl.IsBeingHurt && !PlayerCtrl.IsHealing && !PlayerCtrl.RollCtrl.IsRolling && !PlayerCtrl.IsDialoguing && PlayerCtrl.InputCtrl.Horizontal != 0f);
           
            if (!PlayerCtrl.RollCtrl.IsRolling && !PlayerCtrl.AttackCtrl.IsAttacking && !PlayerCtrl.IsHealing)
                FlipX(flip);

            if (currVel.y < 0f
                && (prevVel.y > 0f
                    || PlayerCtrl.CollisionsCtrl.PreviousStates.GetCollisionState(Templar.Physics.CollisionsController.CollisionOrigin.BELOW)
                    && !PlayerCtrl.CollisionsCtrl.Below)
                && !PlayerCtrl.AttackCtrl.IsAttacking
                && !PlayerCtrl.IsBeingHurt
                && !PlayerCtrl.IsHealing)
            {
                _animator.SetTrigger(FALL);
                LogAnimationPlayIfRequired("Fall");
            }

            if (PlayerCtrl.IsOnMovingPlatform
                && PlayerCtrl.CollisionsCtrl.CurrentStates.GetCollisionState(Templar.Physics.CollisionsController.CollisionOrigin.BELOW)
                && _currStateHash == _fallStateHash)
                PlayIdleAnimation();
        }

        public void PlayAttackAnimation(float dir, Datas.Attack.PlayerAttackDatas attackDatas)
        {
            string attackClipName = string.Format(ATTACK_CLIP_NAME_FORMAT, attackDatas.Id);
            UnityEngine.Assertions.Assert.IsTrue(
                Database.PlayerDatabase.AnimationClips.ContainsKey(attackClipName),
                $"Animation clip {attackClipName} was not found in {Database.PlayerDatabase.Instance.GetType().Name}.");

            OverrideClip(ATTACK_ANM_OVERRIDE_ID, Database.PlayerDatabase.AnimationClips[attackClipName]);

            UpdateAttackAnimation(dir);

            _animator.SetTrigger(ATTACK);
            _animator.SetFloat(MULT_ATTACK, attackDatas.AnimSpeedMult);

            RSLib.Audio.AudioManager.PlaySound(_attackClipProvider);
            
            LogAnimationPlayIfRequired("Attack");
        }

        public void PlayRunAnimation(float dir)
        {
            _animator.SetBool(IS_RUNNING, true);
            FlipX(dir < 0f);
            LogAnimationPlayIfRequired("Run");
        }

        public void StopRunAnimation()
        {
            _animator.SetBool(IS_RUNNING, false);
        }

        public void PlayJumpAnimation(float dir)
        {
            _animator.SetTrigger(JUMP);

            if (dir != 0f)
            {
                Transform jumpPuffInstance = RSLib.Framework.Pooling.Pool.Get(_jumpPuffPrefab).transform;
                jumpPuffInstance.position = transform.position;
                jumpPuffInstance.SetScaleX(dir);
            }
            else
            {
                // We might probably want to use another prefab to have another VFX.
                RSLib.Framework.Pooling.Pool.Get(_landPuffPrefab).transform.position = transform.position;
            }

            RSLib.Audio.AudioManager.PlaySound(_jumpClipProvider);

            LogAnimationPlayIfRequired("Jump");
        }

        public void PlayDoubleJumpAnimation()
        {
            _animator.SetTrigger(JUMP);
            RSLib.Framework.Pooling.Pool.Get(_doubleJumpPuffPrefab).transform.position = transform.position;
            
            RSLib.Audio.AudioManager.PlaySound(_doubleJumpClipProvider);

            LogAnimationPlayIfRequired("Double Jump");
        }

        public void PlayLandAnimation(float velYAbs)
        {
            _animator.SetTrigger(LAND);
            if (velYAbs > _landPuffMinVel)
                PlayLandVFX();

            RSLib.Audio.AudioManager.PlaySound(_landClipProvider);

            LogAnimationPlayIfRequired("Land");
        }

        public void PlaySoftLandAnimation()
        {
            RSLib.Audio.AudioManager.PlaySound(_softLandClipProvider);
            PlayIdleAnimation();
        }
        
        public void PlayLandVFX()
        {
            RSLib.Framework.Pooling.Pool.Get(_landPuffPrefab).transform.position = transform.position;
        }

        public void PlayRollAnimation(float dir)
        {
            FlipX(dir < 0f);
            _animator.SetTrigger(ROLL);
            _animator.SetFloat(MULT_ROLL, PlayerCtrl.CtrlDatas.Roll.AnimMult);

            GameObject rollPuffInstance = RSLib.Framework.Pooling.Pool.Get(_rollPuffPrefab);
            rollPuffInstance.transform.position = transform.position;
            rollPuffInstance.transform.SetScaleX(dir);

            RSLib.Audio.AudioManager.PlaySound(_rollClipProvider);
            
            LogAnimationPlayIfRequired("Roll");
        }

        public void PlayAttackMotionVFX(float dir, float offset, string overrideId = "")
        {
            GameObject smallPuffInstance;

            if (string.IsNullOrEmpty(overrideId))
            {
                smallPuffInstance = Instantiate(_attackPuffPrefab);
            }
            else
            {
                if (!RSLib.Framework.Pooling.Pool.ContainsId(overrideId))
                {
                    CProLogger.LogWarning(this, $"Object Pooler doesn't have a pool with Id {overrideId}. Known Ids are : {string.Join(",", RSLib.Framework.Pooling.Pool.GetPoolsIds())}.");
                    smallPuffInstance = Instantiate(_attackPuffPrefab);
                }
                else
                {
                   smallPuffInstance = RSLib.Framework.Pooling.Pool.Get(overrideId);
                }
            }

            smallPuffInstance.transform.position = transform.position.AddX(-dir * offset);
            smallPuffInstance.transform.SetScaleX(dir);
        }

        public void PlayHitVFX(float dir)
        {
            Transform hitInstance = RSLib.Framework.Pooling.Pool.Get(_hitPrefab).transform;
            hitInstance.position = _hitVFXPivot.position;
            hitInstance.SetScaleX(dir);
        }

        public void PlayHurtAnimation(float dir)
        {
            PlayHurtAnimation();
            BlinkSpriteColor();

            for (int i = _hurtPrefabs.Length - 1; i >= 0; --i)
                Instantiate(_hurtPrefabs[i], transform.position, _hurtPrefabs[i].transform.rotation);

            Transform jumpPuffInstance = RSLib.Framework.Pooling.Pool.Get(_jumpPuffPrefab).transform;
            jumpPuffInstance.position = transform.position;
            jumpPuffInstance.transform.SetScaleX(dir);

            LogAnimationPlayIfRequired("Hurt");
        }

        public void PlayHealAnticipationAnimation()
        {
            _animator.SetTrigger(HEAL_ANTICIPATION);
            LogAnimationPlayIfRequired("Heal Anticipation");
        }

        public void PlayHealAnimation()
        {
            _animator.SetTrigger(HEAL);
            RSLib.Audio.AudioManager.PlaySound(_healClipProvider);
            Manager.GameManager.CameraCtrl.GetShake(Templar.Camera.CameraShake.ID_SMALL).AddTrauma(_healTrauma);
            
            LogAnimationPlayIfRequired("Heal");
        }

        public void PlayCantHealAnimation()
        {
            RSLib.Audio.AudioManager.PlaySound(_cantHealClipProvider);
        }

        public void PlayDialogueIdleAnimation()
        {
            _animator.SetTrigger(DIALOGUE_IDLE);
            LogAnimationPlayIfRequired("Dialogue Idle");
        }

        public void PlayDialogueTalkAnimation()
        {
            // [TODO] Only if NOT currently talking. Use AnimationHash things?
            _animator.SetTrigger(DIALOGUE_TALK);
            LogAnimationPlayIfRequired("Dialogue Talk");
        }

        public void PlayLookUpAnimation()
        {
            _animator.SetBool(LOOK_UP, true);
            LogAnimationPlayIfRequired("Look Up");
        }

        public void PlayLookDownAnimation()
        {
            _animator.SetBool(LOOK_DOWN, true);
            LogAnimationPlayIfRequired("Look Down");
        }

        public void StopLookUpOrDownAnimation()
        {
            _animator.SetBool(LOOK_UP, false);
            _animator.SetBool(LOOK_DOWN, false);
        }

        public override void PlayDeathAnimation(float dir)
        {
            base.PlayDeathAnimation(dir);

            for (int i = _hurtPrefabs.Length - 1; i >= 0; --i)
                Instantiate(_hurtPrefabs[i], transform.position, _hurtPrefabs[i].transform.rotation);

            if (_deathStencilDuration > 0f)
            {
                StencilManager.ShowPlayerStencil(0f);
                StartCoroutine(HideDeathStencilCoroutine());
            }

            LogAnimationPlayIfRequired("Death");
        }

        public void PlayKillTriggerHitAnimation()
        {
            RSLib.Audio.AudioManager.PlaySound(_killTriggerHitClipProvider);
        }

        public void PlayTransitionInAnimation()
        {
            _animator.SetTrigger(TRANSITION_IN);
            LogAnimationPlayIfRequired("Transition In");
        }

        public void PlayTransitionOutAnimation()
        {
            _animator.SetTrigger(TRANSITION_OUT);
            LogAnimationPlayIfRequired("Transition Out");
        }

        private void PlaySleepAnimation()
        {
            _animator.SetTrigger(IDLE_SLEEPING);
            _sleepFeedback.Toggle(true);

            SleepAnimationBegan?.Invoke();
            LogAnimationPlayIfRequired("Idle Sleeping");
        }

        private void UpdateAttackAnimation(float dir = 0f)
        {
            _animator.SetFloat(MULT_ATTACK, PlayerCtrl.AttackCtrl.CurrAttackDatas.AnimSpeedMult);
            if (dir != 0f)
                FlipX(dir < 0f);
        }

        private void UpdateIdleBreakAndSleeping()
        {
            if (Manager.OptionsManager.AnyPanelOpenOrClosedThisFrame())
                return;

            _previousStateHash = _currStateHash;
            _currStateHash = _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

            if (_currStateHash == _idleSleepingStateHash)
                return;

            if (_previousStateHash == _idleSleepingStateHash)
            {
                _sleepFeedback.Toggle(false);
                SleepAnimationOver?.Invoke();
            }

            if (_currStateHash != _idleStateHash)
            {
                if (_currStateHash != _idleBreakStateHash)
                    _idleBreaksCounter = 0;

                _idleBreakTimer = 0f;
                return;
            }

            _idleBreakTimer += Time.deltaTime;
            if (_idleBreakTimer > _idleBreakSecInterval)
            {
                _idleBreaksCounter++;
                _idleBreakTimer = 0f;

                if (_idleBreaksCounter == _breaksBeforeSleep + 1)
                {
                    PlaySleepAnimation();
                }
                else
                {
                    _animator.SetTrigger(IDLE_BREAK);
                    LogAnimationPlayIfRequired("Idle Break");
                }
            }
        }

        private void LogAnimationPlayIfRequired(string animationName)
        {
            if (!_logAnimationsPlays)
                return;

            CProLogger.Log(this, $"Playing animation {animationName}.", gameObject);
        }

        private System.Collections.IEnumerator HideDeathStencilCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_deathStencilDuration);
            StencilManager.HideStencils();
        }

        private void Awake()
        {
            _idleStateHash = Animator.StringToHash("Motion_Idle");
            _idleBreakStateHash = Animator.StringToHash("Idle-Break");
            _idleSleepingStateHash = Animator.StringToHash("Idle-Sleeping");
            _fallStateHash = Animator.StringToHash("Motion_Fall");

            InitAnimatorOverrideController();

            RSLib.Debug.Console.DebugConsole.OverrideCommand("PlayerSleep", "Plays sleep animation.", PlaySleepAnimation);
        }

        private void Update()
        {
            UpdateIdleBreakAndSleeping();
        }
    }
}