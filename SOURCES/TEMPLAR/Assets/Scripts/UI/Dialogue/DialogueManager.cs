namespace Templar.UI.Dialogue
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class DialogueManager : RSLib.Framework.ConsoleProSingleton<DialogueManager>
    {
        [SerializeField] private DialogueView _dialogueView = null;

        private System.Collections.IEnumerator _dialogueCoroutine;
        private bool _skippedSentenceSequence;

        public static void PlayDialogue(string id)
        {
            PlayDialogue(Datas.Dialogue.DialogueDatabase.DialoguesDatas[id]);
        }

        public static void PlayDialogue(Datas.Dialogue.DialogueDatas dialogueDatas)
        {
            Instance._dialogueCoroutine = Instance.PlayDialogueCoroutine(dialogueDatas);
            Instance.StartCoroutine(Instance._dialogueCoroutine);
        }

        public static bool CheckSkipInput()
        {
            // [TODO] Need to set the input somewhere.
            return Input.GetKeyDown(KeyCode.E);
        }

        public static void MarkSentenceAsSkipped()
        {
            Instance._skippedSentenceSequence = true;
        }

        private System.Collections.IEnumerator PlayDialogueCoroutine(Datas.Dialogue.DialogueDatas dialogueDatas)
        {
            Log($"Playing dialogue {dialogueDatas.Id}...");

            for (int i = 0; i < dialogueDatas.SequenceElementsDatas.Length; ++i)
            {
                if (dialogueDatas.SequenceElementsDatas[i] is Datas.Dialogue.SentenceDatas sentenceDatas)
                {
                    yield return PlaySentenceCoroutine(sentenceDatas);
                }
                else if (dialogueDatas.SequenceElementsDatas[i] is Datas.Dialogue.DialoguePauseDatas pauseDatas)
                {
                    // [TODO] Param in xml to hide or not ?
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

            Log($"Dialogue {dialogueDatas.Id} sequence is over.");
        }

        private System.Collections.IEnumerator PlaySentenceCoroutine(Datas.Dialogue.SentenceDatas sentenceDatas)
        {
            _dialogueView.Display(true);
            _dialogueView.ClearText();
            _skippedSentenceSequence = false;

            for (int i = 0; i < sentenceDatas.SequenceElementsDatas.Length; ++i)
            {
                if (sentenceDatas.SequenceElementsDatas[i] is Datas.Dialogue.SentenceTextDatas textDatas)
                {
                    yield return _dialogueView.AppendTextCoroutine(textDatas);
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
                    _dialogueView.SetText(sentenceDatas.SentenceValue);
                    break;
                }
            }

            // If sentence sequence has been skipped, we immediatly want to show the skip feedback, WITHOUT skipping to next sentence.
            if (!_skippedSentenceSequence)
            {
                for (float t = 0f; t <= 1f; t += Time.deltaTime / _dialogueView.SkipInputShowDelay)
                {
                    if (CheckSkipInput())
                        break;

                    yield return null;
                }
            }

            _dialogueView.DisplaySkipInput(true);

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => CheckSkipInput());

            _dialogueView.DisplaySkipInput(false);

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
        }

        private System.Collections.IEnumerator WaitForSentencePause(Datas.Dialogue.SentencePauseDatas pauseDatas)
        {
            if (!pauseDatas.Container.Skippable)
            {
                yield return RSLib.Yield.SharedYields.WaitForSeconds(pauseDatas.Dur);
                yield break;
            }

            for (float t = 0f; t <= 1f; t += Time.deltaTime / pauseDatas.Dur)
            {
                if (CheckSkipInput())
                {
                    MarkSentenceAsSkipped();
                    yield break;
                }

                yield return null;
            }
        }
    }
}