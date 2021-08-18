namespace Templar.Database
{
    using System.Xml.Linq;
    using UnityEngine;

    public class ContextualConditionsDatabase : RSLib.Framework.ConsoleProSingleton<ContextualConditionsDatabase>, IDatabase
    {
        [SerializeField] private TextAsset _contextualConditionsDatas = null;

        public static System.Collections.Generic.Dictionary<string, Datas.ContextualConditions.ContextualConditionsDatas> ContextualConditionsDatas { get; private set; }

        void IDatabase.Load()
        {
            DeserializeContextualConditionsDatas();
        }

        private void DeserializeContextualConditionsDatas()
        {
            XDocument enemiesDatasDoc = XDocument.Parse(_contextualConditionsDatas.text, LoadOptions.SetBaseUri);
            ContextualConditionsDatas = new System.Collections.Generic.Dictionary<string, Datas.ContextualConditions.ContextualConditionsDatas>();

            XElement contextualConditionsDatasElement = enemiesDatasDoc.Element("ContextualConditionsDatas");
            foreach (XElement contextualConditionsElement in contextualConditionsDatasElement.Elements("ContextualConditions"))
            {
                Datas.ContextualConditions.ContextualConditionsDatas contextualConditionsDatas = new Datas.ContextualConditions.ContextualConditionsDatas(contextualConditionsElement);
                ContextualConditionsDatas.Add(contextualConditionsDatas.Id, contextualConditionsDatas);
            }

            Log($"Deserialized {ContextualConditionsDatas.Count} Contextual Conditions datas.");
        }
    }
}