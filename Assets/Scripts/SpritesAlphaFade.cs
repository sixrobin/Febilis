namespace Templar
{
    using RSLib.Extensions;
    using RSLib.Maths;
    using UnityEngine;

    public class SpritesAlphaFade : MonoBehaviour
    {
        [Header("SPRITES")]
        [SerializeField] private SpriteRenderer[] _spriteRenderers = null;

        [Header("DEFAULT VALUES")]
        [SerializeField, Min(0f)] private float _duration = 0.5f;
        [SerializeField, Min(0f)] private Curve _curve = Curve.InOutSine;

        private System.Collections.IEnumerator _fadeCoroutine;

        public void FadeIn()
        {
            FadeIn(_duration, _curve, null);
        }

        public void FadeOut()
        {
            FadeOut(_duration, _curve, null);
        }

        public void FadeIn(float duration, Curve curve, System.Action callback = null)
        {
            if (!gameObject.activeSelf)
                return;

            KillCoroutine();
            StartCoroutine(_fadeCoroutine = FadeCoroutine(true, duration, curve, callback));
        }

        public void FadeOut(float duration, Curve curve, System.Action callback = null)
        {
            if (!gameObject.activeSelf)
                return;

            KillCoroutine();
            StartCoroutine(_fadeCoroutine = FadeCoroutine(false, duration, curve, callback));
        }

        private void KillCoroutine()
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
        }

        private System.Collections.IEnumerator FadeCoroutine(bool fadeIn, float duration, Curve curve, System.Action callback = null)
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                for (int i = _spriteRenderers.Length - 1; i >= 0; --i)
                    _spriteRenderers[i].SetAlpha(fadeIn ? t.Ease(curve) : 1f - t.Ease(curve));
    
                yield return null;
            }

            for (int i = _spriteRenderers.Length - 1; i >= 0; --i)
                _spriteRenderers[i].SetAlpha(fadeIn ? 1f : 0f);

            callback?.Invoke();
        }
    }
}