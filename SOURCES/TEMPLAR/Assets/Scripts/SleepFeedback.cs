namespace Templar
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class SleepFeedback : MonoBehaviour
    {
        [SerializeField] private GameObject _sleepSignPrefab = null;
        [SerializeField, Min(0.1f)] private float _signAppearanceRate = 0.75f;
        [SerializeField, Min(0.1f)] private float _signInitRandomAngle = 60f;
        [SerializeField, Min(0f)] private float _signTravelDist = 1.5f;
        [SerializeField, Min(0f)] private float _signLifespan = 1f;

        private System.Collections.IEnumerator _sleepSignPopCoroutine;
        private bool _isOn;

        public void Toggle(bool on)
        {
            if (_isOn == on)
                return;

            _isOn = on;

            if (_isOn)
                StartCoroutine(_sleepSignPopCoroutine = SpawnSleepSignsCoroutine());
            else
                KillSleepSignPopCoroutine();
        }

        private void KillSleepSignPopCoroutine()
        {
            if (_sleepSignPopCoroutine == null)
                return;

            StopCoroutine(_sleepSignPopCoroutine);
            _sleepSignPopCoroutine = null;
        }

        private System.Collections.IEnumerator SpawnSleepSignsCoroutine()
        {
            while (true)
            {
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_signAppearanceRate);
                StartCoroutine(SleepSignCoroutine(RSLib.Framework.Pooling.Pool.Get(_sleepSignPrefab).transform));
            }
        }

        private System.Collections.IEnumerator SleepSignCoroutine(Transform sign)
        {
            Vector3 dir = Quaternion.Euler(0f, 0f, Random.Range(-_signInitRandomAngle * 0.5f, _signInitRandomAngle * 0.5f)) * transform.up;
            Vector2 initPos = transform.position;
            Vector2 targetPos = transform.position + dir * _signTravelDist;

            sign.transform.localScale = Vector3.zero;

            for (float t = 0f; t < 1f; t += Time.deltaTime / _signLifespan)
            {
                sign.transform.localScale = t < 0.5f ? (Vector3.one * t * 2f) : (Vector3.one * (1f - (t - 0.5f) * 2f));
                sign.transform.position = Vector3.Lerp(initPos, targetPos, t);
                yield return null;
            }

            sign.gameObject.SetActive(false);
        }

        public void DebugToggleOn()
        {
            Toggle(true);
        }

        public void DebugToggleOff()
        {
            Toggle(false);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SleepFeedback))]
    public class SleepFeedbackEditor : RSLib.EditorUtilities.ButtonProviderEditor<SleepFeedback>
    {
        protected override void DrawButtons()
        {
            DrawButton("Toggle On", Obj.DebugToggleOn);
            DrawButton("Toggle Off", Obj.DebugToggleOff);
        }
    }
#endif
}