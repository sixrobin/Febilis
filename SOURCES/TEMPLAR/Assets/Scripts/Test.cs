namespace Templar
{
    using UnityEngine;
    using RSLib.Extensions;

    public class Test : MonoBehaviour, Interaction.Dialogue.ISpeaker
    {
        public string _dialogueId = string.Empty;
        public Transform _dialoguePos = null;

        int switcher = 0;

        public string SpeakerId => "OUI";

        public bool IsDialoguing { get; set; }

        public Vector3 SpeakerPos => transform.position;
        public Vector3 PlayerDialoguePos => _dialoguePos.position;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                UI.Dialogue.DialogueManager.PlayDialogue(_dialogueId, this);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                StartCoroutine(WaitForFramesCoroutine());
            }
        }

        System.Collections.IEnumerator WaitForFramesCoroutine()
        {
            Debug.Log("A");
            yield return RSLib.Yield.SharedYields.WaitForFrames(48);
            Debug.Log("B");
        }

        public void OnSentenceStartOrResume()
        {
            throw new System.NotImplementedException();
        }

        public void OnSentenceStopOrPause()
        {
            throw new System.NotImplementedException();
        }
    }
}