﻿namespace Templar.Unit
{
    using UnityEngine;

    public class UnitView : MonoBehaviour
    {
        private const string IDLE = "Idle";
        private const string DEATH = "Death";
        private const string DEAD_FADE = "Dead_Fade";
        private const string HURT = "Hurt";

        [Header("REFS")]
        [SerializeField] protected SpriteRenderer _spriteRenderer = null;
        [SerializeField] protected Animator _animator = null;
        [SerializeField] protected RSLib.ImageEffects.SpriteBlink _spriteBlink = null;

        public bool GetSpriteRendererFlipX()
        {
            return _spriteRenderer.flipX;
        }

        public void FlipX(bool flip)
        {
            _spriteRenderer.flipX = flip;
        }

        public void BlinkSpriteColor(int count = 1)
        {
            _spriteBlink.BlinkColor(count);
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
    }
}