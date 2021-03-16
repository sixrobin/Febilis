﻿namespace Templar.Unit
{
    using UnityEngine;

    public class UnitView : MonoBehaviour
    {
        private const string DEATH = "Death";
        private const string HURT = "Hurt";

        [Header("REFS")]
        [SerializeField] protected SpriteRenderer _spriteRenderer = null;
        [SerializeField] protected Animator _animator = null;
        [SerializeField] protected RSLib.ImageEffects.SpriteBlink _spriteBlink = null;

        public bool GetSpriteRendererFlipX()
        {
            return _spriteRenderer.flipX;
        }

        public void BlinkSpriteColor(int count = 1)
        {
            _spriteBlink.BlinkColor(count);
        }

        public void BlinkSpriteAlpha(int count = 1)
        {
            _spriteBlink.BlinkAlpha(count);
        }

        public virtual void PlayHurtAnimation(float dir)
        {
            _animator.SetTrigger(HURT);
            BlinkSpriteColor();
        }

        public virtual void PlayDeathAnimation(float dir)
        {
            _spriteRenderer.flipX = dir > 0f;
            _animator.SetTrigger(DEATH);
        }
    }
}