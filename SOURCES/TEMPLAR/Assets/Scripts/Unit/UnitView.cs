namespace Templar.Unit
{
    using UnityEngine;

    public abstract class UnitView : MonoBehaviour
    {
        protected const string IDLE = "Idle";
        protected const string DEATH = "Death";
        protected const string DEAD_FADE = "DeadFade";
        protected const string HURT = "Hurt";
        protected const string MULT_ATTACK = "Mult_Attack";

        [Header("REFS")]
        [SerializeField] protected SpriteRenderer _spriteRenderer = null;
        [SerializeField] protected Animator _animator = null;
        [SerializeField] protected RSLib.ImageEffects.SpriteBlink _spriteBlink = null;

        private System.Collections.IEnumerator _blinkSpriteColorDelayedCoroutine;

        public abstract float DeadFadeDelay { get; }

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

        public virtual void PlayHurtAnimation()
        {
            _animator.SetTrigger(HURT);
        }

        public virtual void PlayDeathAnimation(float dir)
        {
            FlipX(dir > 0f);
            _animator.SetTrigger(DEATH);
        }

        public virtual void PlayDeadFadeAnimation()
        {
            _animator.SetTrigger(DEAD_FADE);
        }

        private System.Collections.IEnumerator BlinkSpriteColorDelayedCoroutine(float delay, int count = 1)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delay);
            _spriteBlink.BlinkColor(count);
        }
    }
}