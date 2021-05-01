namespace Templar.UI.Dialogue
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class DialogueManager : RSLib.Framework.ConsoleProSingleton<DialogueManager>
    {
        [SerializeField] private DialogueView _dialogueView = null;

        private System.Collections.IEnumerator _dialogueCoroutine;
        private string _currDialogueId;

        private bool _skippedSentenceSequence;
        private string _currSentenceProgress;

        public delegate void DialogueEventHandler(string dialogueId);

        public event DialogueEventHandler DialogueStarted;
        public event DialogueEventHandler DialogueOver;

        public static bool DialogueRunning => Instance._dialogueCoroutine != null;

        public static void PlayDialogue(string id, Interaction.Dialogue.ISpeaker sourceSpeaker)
        {
            PlayDialogue(Datas.Dialogue.DialogueDatabase.DialoguesDatas[id], sourceSpeaker);
        }

        public static void PlayDialogue(Datas.Dialogue.DialogueDatas dialogueDatas, Interaction.Dialogue.ISpeaker sourceSpeaker)
        {
            UnityEngine.Assertions.Assert.IsNull(
                Instance._dialogueCoroutine,
                $"Trying to play dialogue {dialogueDatas.Id} while dialogue {Instance._currDialogueId} is already playing.");

            Instance._currDialogueId = dialogueDatas.Id;

            Instance._dialogueCoroutine = Instance.PlayDialogueCoroutine(dialogueDatas, sourceSpeaker);
            Instance.StartCoroutine(Instance._dialogueCoroutine);
        }

        public static bool CheckSkipInput()
        {
            // [TODO] Need to have a better input handling.
            return Input.GetKeyDown(KeyCode.E);
        }

        public static void MarkSentenceAsSkipped()
        {
            Instance._skippedSentenceSequence = true;
        }

        private System.Collections.IEnumerator PlayDialogueCoroutine(Datas.Dialogue.DialogueDatas dialogueDatas, Interaction.Dialogue.ISpeaker sourceSpeaker)
        {
            Log($"Playing dialogue {dialogueDatas.Id}...");

            DialogueStarted?.Invoke(dialogueDatas.Id);

            // We may want to ensure speaker is NOT the player himself, if someday player becomes a speaker.
            Manager.GameManager.PlayerCtrl.IsDialoguing = true;
            yield return Manager.GameManager.PlayerCtrl.PrepareDialogueCoroutine(sourceSpeaker);
            yield return RSLib.Yield.SharedYields.WaitForSeconds(0.5f);
            Manager.GameManager.PlayerCtrl.PlayerView.PlayDialogueIdleAnimation();

            for (int i = 0; i < dialogueDatas.SequenceElementsDatas.Length; ++i)
            {
                if (dialogueDatas.SequenceElementsDatas[i] is Datas.Dialogue.SentenceDatas sentenceDatas)
                {
                    yield return PlaySentenceCoroutine(sentenceDatas);
                }
                else if (dialogueDatas.SequenceElementsDatas[i] is Datas.Dialogue.DialoguePauseDatas pauseDatas)
                {
                    // [TODO] Param in xml to hide panel or not ?
                    _dialogueView.Display(false);
                    yield return RSLib.Yield.SharedYields.WaitForSeconds(pauseDatas.Dur);
                }
                else
                {
                    LogError($"Unhandled dialogue datas type {dialogueDatas.SequenceElementsDatas[i].GetType().Name} encountered during dialogue {dialogueDatas.Id} sequence.");
                    yield break;
                }
            }

            _dialogueView.Display(false);

            Manager.GameManager.PlayerCtrl.IsDialoguing = false;
            Manager.GameManager.PlayerCtrl.PlayerView.PlayIdleAnimation();

            _dialogueCoroutine = null;
            _currDialogueId = string.Empty;

            DialogueOver?.Invoke(dialogueDatas.Id);

            Log($"Dialogue {dialogueDatas.Id} sequence is over.");
        }

        private System.Collections.IEnumerator PlaySentenceCoroutine(Datas.Dialogue.SentenceDatas sentenceDatas)
        {
            _dialogueView.ClearText();
            _dialogueView.SetBoxesPosition(sentenceDatas.PortraitAnchor);
            _dialogueView.SetPortrait(sentenceDatas);
            _dialogueView.Display(true);

            _skippedSentenceSequence = false;
            _currSentenceProgress = string.Empty;

            for (int i = 0; i < sentenceDatas.SequenceElementsDatas.Length; ++i)
            {
                if (sentenceDatas.SequenceElementsDatas[i] is Datas.Dialogue.SentenceTextDatas textDatas)
                {
                    yield return AppendSentenceTextCoroutine(textDatas);
                }
                else if (sentenceDatas.SequenceElementsDatas[i] is Datas.Dialogue.SentencePauseDatas pauseDatas)
                {
                    yield return WaitForSentencePause(pauseDatas);
                }
                else
                {
                    LogError($"Unhandled sentence datas type {sentenceDatas.SequenceElementsDatas[i].GetType().Name} encountered during sentence {sentenceDatas.Id} sequence.");
                    yield break;
                }

                if (_skippedSentenceSequence)
                {
                    _dialogueView.DisplaySentenceProgression(sentenceDatas, sentenceDatas.SentenceValue);
                    break;
                }
            }

            // If sentence sequence has been skipped, we immediatly want to show the skip feedback, WITHOUT skipping to next sentence.
            if (!_skippedSentenceSequence)
                yield return new RSLib.Yield.WaitForSecondsOrBreakIf(_dialogueView.SkipInputShowDelay, CheckSkipInput);

            _dialogueView.DisplaySkipInput(true);

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => CheckSkipInput());

            _dialogueView.DisplaySkipInput(false);

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
                    yield return RSLib.Yield.SharedYields.WaitForSeconds(_dialogueView.TickInterval);
                }
                else
                {
                    yield return new RSLib.Yield.WaitForSecondsOrBreakIf(
                        _dialogueView.TickInterval,
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
                yield return RSLib.Yield.SharedYields.WaitForSeconds(pauseDatas.Dur);
                yield break;
            }

            yield return new RSLib.Yield.WaitForSecondsOrBreakIf(pauseDatas.Dur, CheckSkipInput, MarkSentenceAsSkipped);
        }
    }
}