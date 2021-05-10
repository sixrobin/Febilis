namespace Templar.UI.Dialogue
{
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private RectTransform _portraitBox = null;
        [SerializeField] private RectTransform _textBox = null;
        [SerializeField] private TMPro.TextMeshProUGUI _text = null;
        [SerializeField] private UnityEngine.UI.Image _portrait = null;

        [Header("TEXT TYPING")]
        [SerializeField] private string _speakerSentenceFormat = "{0}: {1}";
        [SerializeField] private int _lettersPerTick = 3; // Overridable in xml ?
        [SerializeField] private float _tickInterval = 0.1f; // Overridable in xml ?

        [Header("SKIP INPUT")]
        [SerializeField] private RectTransform _skipInputFeedback = null;
        [SerializeField] private float _skipInputShowDelay = 1f;
        [SerializeField] private float _skipInputIdleOffset = 1f;
        [SerializeField] private float _skipInputIdleTimestep = 0.25f;

        private System.Collections.IEnumerator _skipInputIdleCoroutine;
        private float _skipInputInitY;

        public int LettersPerTick => _lettersPerTick;
        public float TickInterval => _tickInterval;
        public float SkipInputShowDelay => _skipInputShowDelay;

        public string CurrDisplayedText => _text.text;

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
                StartCoroutine(_skipInputIdleCoroutine = SkipInputIdleCoroutine());
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

        public void DisplaySentenceProgression(Datas.Dialogue.SentenceDatas sentenceDatas, string text)
        {
            if (sentenceDatas.HideSpeakerName)
            {
                _text.text = text;
                return;
            }

            _text.text = string.Format(_speakerSentenceFormat, Datas.Dialogue.DialogueDatabase.GetSpeakerDisplayName(sentenceDatas), text);
        }

        public void SetPortraitAndAnchors(Datas.Dialogue.SentenceDatas sentenceDatas)
        {
            _portrait.sprite = Datas.Dialogue.DialogueDatabase.GetPortraitOrUseDefault(sentenceDatas);

            Datas.Dialogue.PortraitAnchor portraitAnchor = Datas.Dialogue.DialogueDatabase.GetSpeakerPortraitAnchor(sentenceDatas);

            float portraitBoxY = _portraitBox.anchoredPosition.y;
            float textBoxY = _textBox.anchoredPosition.y;

            switch (portraitAnchor)
            {
                case Datas.Dialogue.PortraitAnchor.TOP_RIGHT:
                {
                    _portraitBox.pivot = Vector2.one;
                    _portraitBox.anchorMin = Vector2.one;
                    _portraitBox.anchorMax = Vector2.one;
                    _portraitBox.localScale = new Vector3(-1f, 1f, 1f);
                    _portraitBox.anchoredPosition = new Vector2(-1f - _portraitBox.rect.width, portraitBoxY);

                    _textBox.pivot = new Vector2(0f, 1f);
                    _textBox.anchorMin = new Vector2(0f, 1f);
                    _textBox.anchorMax = new Vector2(0f, 1f);
                    _textBox.anchoredPosition = new Vector2(1f, textBoxY);

                    break;
                }

                case Datas.Dialogue.PortraitAnchor.TOP_LEFT:
                {
                    _portraitBox.pivot = new Vector2(0f, 1f);
                    _portraitBox.anchorMin = new Vector2(0f, 1f);
                    _portraitBox.anchorMax = new Vector2(0f, 1f);
                    _portraitBox.localScale = Vector3.one;
                    _portraitBox.anchoredPosition = new Vector2(1f, portraitBoxY);

                    _textBox.pivot = Vector2.one;
                    _textBox.anchorMin = Vector2.one;
                    _textBox.anchorMax = Vector2.one;
                    _textBox.anchoredPosition = new Vector2(-1f, textBoxY);

                    break;
                }

                default:
                    DialogueManager.Instance.LogError($"Unhandled PortraitAnchor {portraitAnchor} to set dialogue view elements anchors.");
                    break;
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