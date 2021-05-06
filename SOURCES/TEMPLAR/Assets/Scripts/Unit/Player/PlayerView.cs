namespace Templar.Unit.Player
{
    using RSLib.Extensions;
    using UnityEngine;

    public class PlayerView : UnitView
    {
        private const float DEAD_FADE_DELAY = 1.2f;

        private const string IS_RUNNING = "IsRunning";
        private const string IDLE_BREAK = "IdleBreak";
        private const string IDLE_SLEEPING = "IdleSleeping";
        private const string FALL = "Fall";
        private const string JUMP = "Jump";
        private const string LAND = "Land";
        private const string ROLL = "Roll";
        private const string ATTACK = "Attack";
        private const string ATTACK_CHAIN = "AttackChain";
        private const string ATTACK_AIRBORNE = "AttackAirborne";
        private const string HEAL_ANTICIPATION = "HealAnticipation";
        private const string HEAL = "Heal";
        private const string DIALOGUE_IDLE = "DialogueIdle";
        private const string DIALOGUE_TALK = "DialogueTalk";
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

        [Header("DEBUG")]
        [SerializeField] private bool _logAnimationsPlays = false;

        private int _idleStateHash;
        private int _idleBreakStateHash;
        private int _idleSleepingStateHash;

        private int _currStateHash;
        private int _previousStateHash;

        private float _idleBreakTimer;
        private int _idleBreaksCounter;

        public PlayerController TemplarController { get; set; }

        public override float DeadFadeDelay => DEAD_FADE_DELAY;

        public void UpdateView(bool flip, Vector3 currVel, Vector3 prevVel)
        {
            _animator.SetBool(IS_RUNNING, !TemplarController.IsBeingHurt && !TemplarController.RollCtrl.IsRolling && TemplarController.InputCtrl.Horizontal != 0f);

            if (!TemplarController.RollCtrl.IsRolling && !TemplarController.AttackCtrl.IsAttacking)
                FlipX(flip);

            if (currVel.y < 0f
                && (prevVel.y > 0f
                || TemplarController.CollisionsCtrl.PreviousStates.GetCollisionState(Templar.Physics.CollisionsController.CollisionOrigin.BELOW)
                && !TemplarController.CollisionsCtrl.Below)
                && !TemplarController.AttackCtrl.IsAttacking
                && !TemplarController.IsBeingHurt)
                _animator.SetTrigger(FALL);
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
                GameObject jumpPuffInstance = Instantiate(_jumpPuffPrefab, transform.position, _jumpPuffPrefab.transform.rotation);
                jumpPuffInstance.transform.SetScaleX(dir);
            }
            else
            {
                // We might probably want to use another prefab to have another VFX.s
                Instantiate(_landPuffPrefab, transform.position, _landPuffPrefab.transform.rotation);
            }

            LogAnimationPlayIfRequired("Jump");
        }

        public void PlayDoubleJumpAnimation()
        {
            _animator.SetTrigger(JUMP);
            Instantiate(_doubleJumpPuffPrefab, transform.position, _doubleJumpPuffPrefab.transform.rotation);
            LogAnimationPlayIfRequired("Double Jump");
        }

        public void PlayLandAnimation(float velYAbs)
        {
            _animator.SetTrigger(LAND);
            if (velYAbs > _landPuffMinVel)
                PlayLandVFX();

            LogAnimationPlayIfRequired("Land");
        }

        public void PlayLandVFX()
        {
            Instantiate(_landPuffPrefab, transform.position, _landPuffPrefab.transform.rotation);
        }

        public void PlayRollAnimation(float dir)
        {
            FlipX(dir < 0f);
            _animator.SetTrigger(ROLL);
            _animator.SetFloat(MULT_ROLL, TemplarController.CtrlDatas.Roll.AnimMult);

            GameObject rollPuffInstance = Instantiate(_rollPuffPrefab, transform.position, _rollPuffPrefab.transform.rotation);
            rollPuffInstance.transform.SetScaleX(dir);

            LogAnimationPlayIfRequired("Roll");
        }

        public void PlayAttackAnimation(float dir)
        {
            UpdateAttackAnimation(dir);
            _animator.SetTrigger(ATTACK);
            LogAnimationPlayIfRequired("Attack");
        }

        public void PlayChainAttackAnimation(float dir)
        {
            UpdateAttackAnimation(dir);
            _animator.SetTrigger(ATTACK_CHAIN);
            LogAnimationPlayIfRequired("Chain Attack");
        }

        public void PlayAttackAirborneAnimation()
        {
            UpdateAttackAnimation();
            _animator.SetTrigger(ATTACK_AIRBORNE);
            LogAnimationPlayIfRequired("Airborne Attack");
        }

        public void PlayAttackVFX(float dir, float offset)
        {
            GameObject smallPuffInstance = Instantiate(_attackPuffPrefab, transform.position - new Vector3(dir * offset, 0f), _attackPuffPrefab.transform.rotation);
            smallPuffInstance.transform.SetScaleX(dir);
        }

        public void PlayHitVFX(float dir)
        {
            GameObject hitInstance = Instantiate(_hitPrefab, _hitVFXPivot.position, _hitPrefab.transform.rotation);
            hitInstance.transform.SetScaleX(dir);
        }

        public void PlayHurtAnimation(float dir)
        {
            PlayHurtAnimation();
            BlinkSpriteColor();

            for (int i = _hurtPrefabs.Length - 1; i >= 0; --i)
                Instantiate(_hurtPrefabs[i], transform.position, _hurtPrefabs[i].transform.rotation);

            // [TMP] We probably want a puff VFX made especially for hurt feedback.
            GameObject jumpPuffInstance = Instantiate(_jumpPuffPrefab, transform.position, _jumpPuffPrefab.transform.rotation);
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
            LogAnimationPlayIfRequired("Heal");
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

        public override void PlayDeathAnimation(float dir)
        {
            base.PlayDeathAnimation(dir);

            for (int i = _hurtPrefabs.Length - 1; i >= 0; --i)
                Instantiate(_hurtPrefabs[i], transform.position, _hurtPrefabs[i].transform.rotation);

            LogAnimationPlayIfRequired("Death");
        }

        private void UpdateAttackAnimation(float dir = 0f)
        {
            _animator.SetFloat(MULT_ATTACK, TemplarController.AttackCtrl.CurrAttackDatas.AnimSpeedMult);
            if (dir != 0f)
                FlipX(dir < 0f);
        }

        private void UpdateIdleBreakAndSleeping()
        {
            _previousStateHash = _currStateHash;
            _currStateHash = _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

            if (_currStateHash == _idleSleepingStateHash)
                return;
            else if (_previousStateHash == _idleSleepingStateHash)
                _sleepFeedback.Toggle(false);

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
                    _animator.SetTrigger(IDLE_SLEEPING);
                    _sleepFeedback.Toggle(true);
                    LogAnimationPlayIfRequired("Idle Sleeping");
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

        private void Awake()
        {
            _idleStateHash = Animator.StringToHash("Motion_Idle");
            _idleBreakStateHash = Animator.StringToHash("Idle-Break");
            _idleSleepingStateHash = Animator.StringToHash("Idle-Sleeping");
        }

        private void Update()
        {
            UpdateIdleBreakAndSleeping();
        }
    }
}