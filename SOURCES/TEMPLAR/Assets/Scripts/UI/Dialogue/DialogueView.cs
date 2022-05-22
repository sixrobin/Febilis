namespace Templar.UI.Dialogue
{
    using RSLib.Extensions;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class DialogueView : MonoBehaviour
    {
        private const string ALPHA_TAG_OPEN = "<color=#00000000>";
        private const string ALPHA_TAG_CLOSE = "</color>";
        
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private UnityEngine.UI.CanvasScaler _canvasScaler = null;
        [SerializeField] private RectTransform _portraitBox = null;
        [SerializeField] private RectTransform _textBox = null;
        [SerializeField] private TMPro.TextMeshProUGUI _text = null;
        [SerializeField] private UnityEngine.UI.Image _portrait = null;

        [Header("TEXT TYPING")]
        [SerializeField] private string _speakerSentenceFormat = "{0}: {1}";
        [SerializeField] private int _lettersPerTick = 3;
        [SerializeField] private float _tickInterval = 0.1f;

        [Header("SKIP INPUT")]
        [SerializeField] private RectTransform _skipInputFeedback = null;
        [SerializeField] private float _skipInputShowDelay = 1f;
        [SerializeField] private float _skipInputIdleOffset = 1f;
        [SerializeField] private float _skipInputIdleTimestep = 0.25f;

        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _characterTypedClipProvider = null;
        [SerializeField] private RSLib.Audio.ClipProvider _skipFeedbackShowClipProvider = null;
        
        private System.Collections.IEnumerator _skipInputIdleCoroutine;
        private float _skipInputInitY;

        private float _textBoxInitWidth;
        private string _currSentence;
        
        public int LettersPerTick => _lettersPerTick;
        public float TickInterval => _tickInterval;
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

        public void PrepareSentence(string text)
        {
            _currSentence = text;
            _text.text = ALPHA_TAG_OPEN + _currSentence + ALPHA_TAG_CLOSE;
        }

        private string[] SplitSentence(string sentence, int firstChunkLength)
        {
            string[] splitSentence = new string[2];

            for (int i = 0; i < firstChunkLength; ++i)
                splitSentence[0] += sentence[i];

            for (int i = firstChunkLength; i < sentence.Length; ++i)
                splitSentence[1] += sentence[i];

            return splitSentence;
        }
        
        public void DisplaySentenceProgression(Datas.Dialogue.SentenceTextDatas sentenceTextData, int progression)
        {
            string displaySentence;
            
            if (progression == -1)
            {
                displaySentence = _currSentence;
            }
            else
            {
                string[] splitSentence = SplitSentence(_currSentence, progression);
                displaySentence = splitSentence[0] + ALPHA_TAG_OPEN + splitSentence[1] + ALPHA_TAG_CLOSE;
            }

            _text.text = sentenceTextData.Container.HideSpeakerName
                         ? displaySentence
                         : string.Format(_speakerSentenceFormat, Database.DialogueDatabase.GetSpeakerDisplayName(sentenceTextData.Container), displaySentence);
            
            RSLib.Audio.AudioManager.PlaySound(_characterTypedClipProvider);
        }

        public void SetPortraitDisplay(bool show)
        {
            _portraitBox.gameObject.SetActive(show);

            _textBox.sizeDelta = _textBox.sizeDelta.WithX(show
                                                          ? _textBoxInitWidth
                                                          : _canvasScaler.referenceResolution.x - Mathf.Abs(_textBox.anchoredPosition.x * 2));
        }

        public void SetPortraitAndAnchors(Datas.Dialogue.SentenceDatas sentenceData, bool invertAnchors)
        {
            _portrait.sprite = Database.DialogueDatabase.GetPortraitOrUseDefault(sentenceData);

            Datas.Dialogue.PortraitAnchor portraitAnchor = Database.DialogueDatabase.GetSpeakerPortraitAnchor(sentenceData);
            if (invertAnchors)
                portraitAnchor = Datas.Dialogue.PortraitAnchorExtensions.GetOpposite(portraitAnchor);

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

            RSLib.Audio.AudioManager.PlaySound(_skipFeedbackShowClipProvider);
            
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
            _textBoxInitWidth = _textBox.rect.width;
        }
    }
}