namespace Templar.UI.Dialogue
{
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class DialogueManager : RSLib.Framework.ConsoleProSingleton<DialogueManager>
    {
        [SerializeField] private DialogueView _dialogueView = null;
        [SerializeField] private RSLib.Framework.DisabledString _currentDialogueId = new RSLib.Framework.DisabledString();
        [SerializeField, Min(0f)] private float _delayBeforeFirstDialogueElement = 0.5f;
        [SerializeField, Min(0f)] private float _goToPositionTimeout = 3f;

        [Header("DEBUG")]
        [SerializeField] private bool _debugFastDialogues = false;

        private System.Collections.Generic.Dictionary<string, Interaction.Dialogue.ISpeaker> _speakers;

        private System.Collections.IEnumerator _dialogueCoroutine;
        private Datas.Dialogue.DialogueDatas _currentDialogue;

        private bool _skippedSentenceSequence;
        private string _currSentenceProgress;

        public delegate void DialogueEventHandler(Datas.Dialogue.DialogueDatas dialogueDatas);

        public event DialogueEventHandler DialogueStarted;
        public event DialogueEventHandler DialogueOver;

        public static bool DialogueRunning => Exists() && Instance._dialogueCoroutine != null;

        public static void PlayDialogue(string id, Interaction.Dialogue.ISpeaker sourceSpeaker = null)
        {
            PlayDialogue(Database.DialogueDatabase.DialoguesDatas[id], sourceSpeaker);
        }

        public static void PlayDialogue(Datas.Dialogue.DialogueDatas dialogueDatas, Interaction.Dialogue.ISpeaker sourceSpeaker = null)
        {
            UnityEngine.Assertions.Assert.IsFalse(
                DialogueRunning,
                $"Trying to play dialogue {dialogueDatas.Id} while dialogue {Instance._currentDialogue?.Id} is already playing.");

            Instance._currentDialogue = dialogueDatas;
            Instance._currentDialogueId = new RSLib.Framework.DisabledString(dialogueDatas.Id);

            Instance.StartCoroutine(Instance._dialogueCoroutine = Instance.PlayDialogueCoroutine(sourceSpeaker));
        }

        public static bool CheckSkipInput()
        {
            return RSLib.Framework.InputSystem.InputManager.GetAnyInputDown(
                Unit.Player.PlayerInputController.JUMP,
                Unit.Player.PlayerInputController.INTERACT,
                Unit.Player.PlayerInputController.ATTACK);
        }

        public static void MarkSentenceAsSkipped()
        {
            Instance._skippedSentenceSequence = true;
        }

        private void RegisterSpeakersInScene()
        {
            _speakers = new System.Collections.Generic.Dictionary<string, Interaction.Dialogue.ISpeaker>();
            System.Collections.Generic.IEnumerable<Interaction.Dialogue.ISpeaker> speakers =
                RSLib.Helpers.FindInstancesOfType<Interaction.Dialogue.ISpeaker>().Where(o => !string.IsNullOrEmpty(o.SpeakerId));
            
            foreach (Interaction.Dialogue.ISpeaker speaker in speakers)
                _speakers.Add(speaker.SpeakerId, speaker);

            Log($"Registered {_speakers.Count} speaker(s) is scene : {string.Join(",", _speakers.Keys)}.");
        }

        private System.Collections.IEnumerator PlayDialogueCoroutine(Interaction.Dialogue.ISpeaker sourceSpeaker = null)
        {
            Log($"Playing dialogue {_currentDialogue.Id}...");
            DialogueStarted?.Invoke(_currentDialogue);

            Manager.GameManager.PlayerCtrl.IsDialoguing = true;

            if (sourceSpeaker is Interaction.Dialogue.INPCSpeaker npcSpeaker)
                yield return Manager.GameManager.PlayerCtrl.GoToInteractionPosition(npcSpeaker.SpeakerPos, npcSpeaker.PlayerDialoguePivot, _goToPositionTimeout);

            Manager.GameManager.PlayerCtrl.PlayerView.PlayIdleAnimation();
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_delayBeforeFirstDialogueElement);
            Manager.GameManager.PlayerCtrl.PlayerView.PlayDialogueIdleAnimation();

            _dialogueView.SetPortraitDisplay(!_currentDialogue.HidePortraitBox);

            for (int i = 0; i < _currentDialogue.SequenceElementsDatas.Length; ++i)
            {
                if (_currentDialogue.SequenceElementsDatas[i] is Datas.Dialogue.SentenceDatas sentenceDatas)
                {
                    yield return PlaySentenceCoroutine(sentenceDatas);
                }
                else if (_currentDialogue.SequenceElementsDatas[i] is Datas.Dialogue.DialoguePauseDatas pauseDatas)
                {
                    _dialogueView.Display(false);
                    yield return RSLib.Yield.SharedYields.WaitForSeconds(pauseDatas.Dur);
                }
                else if (_currentDialogue.SequenceElementsDatas[i] is Datas.Dialogue.DialogueAddItemDatas addItemDatas)
                {
                    Manager.GameManager.InventoryCtrl.AddItem(addItemDatas.ItemId, addItemDatas.Quantity);
                }
                else if (_currentDialogue.SequenceElementsDatas[i] is Datas.Dialogue.DialogueRemoveItemDatas removeItemDatas)
                {
                    Manager.GameManager.InventoryCtrl.RemoveItem(removeItemDatas.ItemId, removeItemDatas.Quantity);
                }
                else
                {
                    LogError($"Unhandled dialogue data type {_currentDialogue.SequenceElementsDatas[i].GetType().Name} encountered during dialogue {_currentDialogue.Id} sequence.");
                    yield break;
                }
            }

            _dialogueView.Display(false);

            Manager.GameManager.PlayerCtrl.IsDialoguing = false;
            Manager.GameManager.PlayerCtrl.PlayerView.PlayIdleAnimation();

            DialogueOver?.Invoke(_currentDialogue);
            Log($"Dialogue {_currentDialogue.Id} sequence is over.");

            _dialogueCoroutine = null;
            _currentDialogue = null;
        }

        private System.Collections.IEnumerator PlaySentenceCoroutine(Datas.Dialogue.SentenceDatas sentenceData)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                _speakers.ContainsKey(sentenceData.SpeakerId),
                $"Speaker Id {sentenceData.SpeakerId} is not known by DialogueManager. Known speakers are {string.Join(",", _speakers.Keys)}.");

            _dialogueView.ClearText();

            bool displayPortraitBox = !_currentDialogue.HidePortraitBox && !sentenceData.HidePortraitBox;
            _dialogueView.SetPortraitDisplay(displayPortraitBox);
            if (displayPortraitBox)
                _dialogueView.SetPortraitAndAnchors(sentenceData, _currentDialogue.InvertPortraitsAnchors);
            
            _dialogueView.Display(true);

            _skippedSentenceSequence = false;
            _currSentenceProgress = string.Empty;

            _speakers[sentenceData.SpeakerId].OnSentenceStart();

            Datas.Dialogue.SentenceSequenceElementDatas[] sequenceElementsData = sentenceData.SequenceElementsDatasByLanguage[Localizer.Instance.Language];
            
            for (int i = 0; i < sequenceElementsData.Length; ++i)
            {
                if (sequenceElementsData[i] is Datas.Dialogue.SentenceTextDatas textData)
                {
                    yield return AppendSentenceTextCoroutine(textData);
                }
                else if (sequenceElementsData[i] is Datas.Dialogue.SentencePauseDatas pauseData)
                {
                    yield return WaitForSentencePause(pauseData);
                }
                else
                {
                    LogError($"Unhandled sentence data type {sequenceElementsData[i].GetType().Name} encountered during sentence {sentenceData.Id} sequence.");
                    yield break;
                }

                if (_skippedSentenceSequence)
                {
                    _dialogueView.DisplaySentenceProgression(sentenceData, sentenceData.SentenceValueByLanguage[Localizer.Instance.Language]);
                    break;
                }
            }

            // If sentence sequence has been skipped, we immediately want to show the skip feedback, WITHOUT skipping to next sentence.
            if (!_skippedSentenceSequence && !_debugFastDialogues)
                yield return new RSLib.Yield.WaitForSecondsOrBreakIf(_dialogueView.SkipInputShowDelay, CheckSkipInput);

            _dialogueView.DisplaySkipInput(true);

            // Double skip input issue in build fixed with this 3 frames wait.
            for (int i = 0; i < 3; ++i)
                yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            
            yield return new WaitUntil(() => CheckSkipInput() || _debugFastDialogues);

            _dialogueView.DisplaySkipInput(false);
            _speakers[sentenceData.SpeakerId].OnSentenceEnd();

            // Double skip input issue in build fixed with this 3 frames wait.
            for (int i = 0; i < 3; ++i)
                yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
        }

        private System.Collections.IEnumerator AppendSentenceTextCoroutine(Datas.Dialogue.SentenceTextDatas textDatas)
        {
            string initStr = _currSentenceProgress;

            int i = 0;
            while (i < textDatas.Value.Length)
            {
                int substringLength = Mathf.Min(_dialogueView.LettersPerTick, textDatas.Value.Length - i);
                _currSentenceProgress += textDatas.Value.Substring(i, substringLength);

                if (!textDatas.Container.Skippable)
                {
                    yield return RSLib.Yield.SharedYields.WaitForSeconds(_debugFastDialogues ? _dialogueView.TickInterval / 3f : _dialogueView.TickInterval);
                }
                else
                {
                    yield return new RSLib.Yield.WaitForSecondsOrBreakIf(
                        _debugFastDialogues ? _dialogueView.TickInterval / 3f : _dialogueView.TickInterval,
                        CheckSkipInput,
                        () =>
                        {
                            _currSentenceProgress = initStr + textDatas.Value;
                            _dialogueView.DisplaySentenceProgression(textDatas.Container, _currSentenceProgress);
                            MarkSentenceAsSkipped();
                        });

                    if (_skippedSentenceSequence)
                        yield break;
                }

                _dialogueView.DisplaySentenceProgression(textDatas.Container, _currSentenceProgress);
                i += _dialogueView.LettersPerTick;
            }
        }

        private System.Collections.IEnumerator WaitForSentencePause(Datas.Dialogue.SentencePauseDatas pauseDatas)
        {
            if (!pauseDatas.Container.Skippable)
            {
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_debugFastDialogues ? pauseDatas.Dur / 3f : pauseDatas.Dur);
                yield break;
            }

            yield return new RSLib.Yield.WaitForSecondsOrBreakIf(
                _debugFastDialogues ? pauseDatas.Dur / 3f : pauseDatas.Dur,
                () => CheckSkipInput() || _debugFastDialogues,
                MarkSentenceAsSkipped);
        }

        protected override void Awake()
        {
            base.Awake();
            RegisterSpeakersInScene();

            if (_dialogueView == null)
            {
                Instance.LogError($"{nameof(DialogueView)} reference is missing on {Instance.GetType().Name}, getting it through FindObjectOfType!");
                _dialogueView = FindObjectOfType<DialogueView>();
            }

            RSLib.Debug.Console.DebugConsole.OverrideCommand<string>("PlayDialogue", "Plays a dialogue by Id.", (id) => PlayDialogue(id));
            RSLib.Debug.Console.DebugConsole.OverrideCommand("ToggleFastDialogues", "Toggles dialogue light speed.", () => _debugFastDialogues = !_debugFastDialogues);
            RSLib.Debug.Console.DebugConsole.OverrideCommand<bool>("ToggleFastDialogues", "Set dialogue light speed state.", (state) => _debugFastDialogues = state);
        }

        [ContextMenu("Find All References")]
        private void DebugFindAllReferences()
        {
            _dialogueView = FindObjectOfType<DialogueView>();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }

        [ContextMenu("Find Missing References")]
        private void DebugFindMissingReferences()
        {
            _dialogueView = _dialogueView ?? FindObjectOfType<DialogueView>();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
    }
}