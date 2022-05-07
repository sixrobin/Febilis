namespace Templar.Unit
{
    using RSLib.Extensions;
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    public abstract class UnitView : MonoBehaviour
    {
        protected const string IDLE = "Idle";
        protected const string DEATH = "Death";
        protected const string DEAD_FADE = "DeadFade";
        protected const string HURT = "Hurt";
        protected const string STUN = "Stun";
        protected const string ATTACK = "Attack";
        protected const string MULT_WALK = "Mult_Walk";
        protected const string MULT_ATTACK = "Mult_Attack";

        [Header("REFS")]
        [SerializeField] protected SpriteRenderer _spriteRenderer = null;
        [SerializeField] protected Animator _animator = null;
        [SerializeField] protected RSLib.ImageEffects.SpriteBlink _spriteBlink = null;
        [SerializeField] protected GameObject _stunStars = null;

        [Header("FEEDBACK")]
        [SerializeField] protected float _deathFreezeFrameDuration = 0.1f;

        [Header("AOC")]
        [SerializeField] private AnimatorOverrideController _aocTemplate = null;

        [Header("AUDIO - UNIT BASE")]
        [SerializeField] protected RSLib.Audio.ClipProvider _attackClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _footstepClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _hurtClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _stunClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _deathClipProvider = null;

        private System.Collections.IEnumerator _blinkSpriteColorDelayedCoroutine;

        private AnimatorOverrideController _aoc;
        private List<KeyValuePair<AnimationClip, AnimationClip>> _initClips;

        private float _stunStarsInitX;

        public abstract float DeadFadeDelay { get; }

        public SpriteRenderer Renderer => _spriteRenderer;

#region ANIMATION EVENTS

        public void OnFootstep()
        {
            RSLib.Audio.AudioManager.PlaySound(_footstepClipProvider);
        }

#endregion // ANIMATION EVENTS
        
        public bool GetSpriteRendererFlipX()
        {
            return _spriteRenderer.flipX;
        }

        public void FlipX(bool flip)
        {
            _spriteRenderer.flipX = flip;
        }

        public void BlinkSpriteColor(float delay = 0f, int count = 1)
        {
            if (delay == 0f)
            {
                _spriteBlink.BlinkColor(count);
                return;
            }

            if (_blinkSpriteColorDelayedCoroutine != null)
            {
                StopCoroutine(_blinkSpriteColorDelayedCoroutine);
                _spriteBlink.ResetColor();
            }

            _blinkSpriteColorDelayedCoroutine = BlinkSpriteColorDelayedCoroutine(delay, count);
            StartCoroutine(_blinkSpriteColorDelayedCoroutine);
        }

        public void BlinkSpriteAlpha(int count = 1)
        {
            _spriteBlink.BlinkAlpha(count);
        }

        public void PlayIdleAnimation()
        {
            _animator.SetTrigger(IDLE);
        }

        public void PlayHurtAnimation()
        {
            _animator.SetTrigger(HURT);
            RSLib.Audio.AudioManager.PlaySound(_hurtClipProvider);
        }

        public void PlayStunAnimation(float dir)
        {
            _animator.SetTrigger(STUN);

            if (_stunStars != null)
            {
                _stunStars.transform.SetLocalPositionX(_stunStarsInitX * dir);
                _stunStars.SetActive(true);
            }
        }

        public void PlayStunAudio()
        {
            RSLib.Audio.AudioManager.PlaySound(_stunClipProvider);
        }

        public virtual void OnStunAnimationOver()
        {
            if (_stunStars != null)
                _stunStars.SetActive(false);
        }

        public virtual void PlayDeathAnimation(float dir)
        {
            FlipX(dir > 0f);
            _animator.SetTrigger(DEATH);
            
            Manager.FreezeFrameManager.FreezeFrame(0, _deathFreezeFrameDuration);
            RSLib.Audio.AudioManager.PlaySound(_deathClipProvider);
        }

        public void PlayDeadFadeAnimation()
        {
            _animator.SetTrigger(DEAD_FADE);
        }

        public void SetWalkMultiplier(float value)
        {
            _animator.SetFloat(MULT_WALK, value);
        }
        
        protected void InitAnimatorOverrideController()
        {
            _aoc = new AnimatorOverrideController(_aocTemplate.runtimeAnimatorController) { name = $"aocCopy_{transform.name}" };

            List<KeyValuePair<AnimationClip, AnimationClip>> clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            _aocTemplate.GetOverrides(clips);
            _aoc.ApplyOverrides(clips);
            _initClips = clips;

            _animator.runtimeAnimatorController = _aoc;
        }

        protected void OverrideClip(string key, AnimationClip clip)
        {
            _aoc[key] = clip;
        }

        protected void RestoreInitClip(string key)
        {
            foreach (KeyValuePair<AnimationClip, AnimationClip> initClip in _initClips)
                if (initClip.Key.name == key)
                    _aoc[key] = initClip.Value;
        }

        private System.Collections.IEnumerator BlinkSpriteColorDelayedCoroutine(float delay, int count = 1)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delay);
            _spriteBlink.BlinkColor(count);
        }

        protected virtual void Start()
        {
            if (_stunStars != null)
                _stunStarsInitX = _stunStars.transform.localPosition.x;
        }
    }
}