namespace Templar
{
    using UnityEngine;
    using RSLib.Extensions;

    public class Test : MonoBehaviour
    {
        public string _dialogueId = string.Empty;

        // Start is called before the first frame update
        void Start() 
        {

        }

        int switcher = 0;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                UI.Dialogue.DialogueManager.PlayDialogue(_dialogueId);
            }
        }
    }
}