namespace Templar.UI.Dialogue
{
    using RSLib.Extensions;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private TMPro.TextMeshProUGUI _text = null;
        [SerializeField] private RectTransform _skipInputFeedback = null;
        [SerializeField] private int _lettersPerTick = 3; // Overridable in xml ?
        [SerializeField] private float _tickInterval = 0.1f; // Overridable in xml ?

        [Header("SKIP INPUT")]
        [SerializeField] private float _skipInputShowDelay = 1f;
        [SerializeField] private float _skipInputIdleOffset = 1f;
        [SerializeField] private float _skipInputIdleTimestep = 0.25f;

        private System.Collections.IEnumerator _skipInputIdleCoroutine;
        private float _skipInputInitY;

        public float SkipInputShowDelay => _skipInputShowDelay;

        public void Display(bool show)
        {
            _canvas.enabled = show;
            DisplaySkipInput(false);
        }

        public void DisplaySkipInput(bool show)
        {
            if (show)
            {
                UnityEngine.Assertions.Assert.IsNull(_skipInputIdleCoroutine, "Skip input is being displayed but its idle coroutine is already running.");

                _skipInputIdleCoroutine = SkipInputIdleCoroutine();
                StartCoroutine(_skipInputIdleCoroutine);
                return;
            }

            if (_skipInputIdleCoroutine != null)
            {
                StopCoroutine(_skipInputIdleCoroutine);
                _skipInputIdleCoroutine = null;
            }

            _skipInputFeedback.gameObject.SetActive(false);
        }

        public void ClearText()
        {
            _text.text = string.Empty;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        // [TODO] Move this in DialogueManager. This should not handle skipping or calling manager methods.
        public System.Collections.IEnumerator AppendTextCoroutine(Datas.Dialogue.SentenceTextDatas textDatas)
        {
            string initStr = _text.text;
            string str = initStr;

            int i = 0;
            while (i < textDatas.Value.Length)
            {
                int substringLength = Mathf.Min(_lettersPerTick, textDatas.Value.Length - i);
                str += textDatas.Value.Substring(i, substringLength);

                if (!textDatas.Container.Skippable)
                {
                    yield return RSLib.Yield.SharedYields.WaitForSeconds(_tickInterval);
                }
                else
                {
                    for (float t = 0f; t <= 1f; t += Time.deltaTime / _tickInterval)
                    {
                        if (DialogueManager.CheckSkipInput())
                        {
                            _text.text = initStr + textDatas.Value;
                            DialogueManager.MarkSentenceAsSkipped();
                            yield break;
                        }

                        yield return null;
                    }
                }

                _text.text = str;
                i += _lettersPerTick;
            }
        }

        private System.Collections.IEnumerator SkipInputIdleCoroutine()
        {
            _skipInputFeedback.gameObject.SetActive(true);
            _skipInputFeedback.anchoredPosition = new Vector2(_skipInputFeedback.anchoredPosition.x, _skipInputInitY);
            int sign = 0;

            while (true)
            {
                _skipInputFeedback.anchoredPosition += new Vector2(0f, ++sign % 2 == 0 ? _skipInputIdleOffset : -_skipInputIdleOffset);
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_skipInputIdleTimestep);
            }
        }

        private void Awake()
        {
            Display(false);
            _skipInputInitY = _skipInputFeedback.anchoredPosition.y;
        }
    }
}