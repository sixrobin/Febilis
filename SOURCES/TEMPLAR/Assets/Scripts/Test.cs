namespace Templar
{
    using UnityEngine;
    using RSLib.Extensions;

    public class Test : MonoBehaviour
    {
        public string _sentenceId = string.Empty;

        // Start is called before the first frame update
        void Start() 
        {
            Datas.Dialogue.SentenceDatas sentence = Datas.Dialogue.DialogueDatabase.SentencesDatas[_sentenceId];

            for (int i = 0; i < sentence.SequenceElementsDatas.Length; ++i)
            {
                if (sentence.SequenceElementsDatas[i] is Datas.Dialogue.SentenceTextDatas text)
                {
                    Debug.Log(text.Value);
                }
                else if (sentence.SequenceElementsDatas[i] is Datas.Dialogue.SentencePauseDatas pause)
                {
                    Debug.Log(pause.Dur);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}