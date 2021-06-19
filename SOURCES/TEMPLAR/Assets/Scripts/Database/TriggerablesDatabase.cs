namespace Templar.Database
{
    using System.Xml.Linq;
    using UnityEngine;

    public class TriggerablesDatabase : RSLib.Framework.ConsoleProSingleton<TriggerablesDatabase>, IDatabase
    {
        [SerializeField] private TextAsset _triggerablesDatas = null;

        public static System.Collections.Generic.Dictionary<string, Datas.TriggerableDatas> TriggerablesDatas { get; private set; }

        void IDatabase.Load()
        {
            DeserializeTriggerablesDatas();
        }

        private void DeserializeTriggerablesDatas()
        {
            TriggerablesDatas = new System.Collections.Generic.Dictionary<string, Datas.TriggerableDatas>();

            XDocument triggerablesDatasDoc = XDocument.Parse(_triggerablesDatas.text, LoadOptions.SetBaseUri);
            XElement triggerablesDatasElement = triggerablesDatasDoc.Element("TriggerablesDatas");

            foreach (XElement triggerableDatasElement in triggerablesDatasElement.Elements("TriggerableDatas"))
            {
                Datas.TriggerableDatas triggerableDatas = new Datas.TriggerableDatas(triggerableDatasElement);
                TriggerablesDatas.Add(triggerableDatas.Id, triggerableDatas);
            }

            Log($"Deserialized {TriggerablesDatas.Count} triggerable objects datas.");
        }
    }
}