namespace Templar.Dev
{
    using System.Xml.Linq;
    using UnityEngine;

    public class EnemyDatabase : RSLib.Framework.Singleton<EnemyDatabase>
    {
        [SerializeField] private TextAsset _enemyDefinitions = null;

        public static System.Collections.Generic.Dictionary<string, EnemyDefinition> EnemyDefinitions { get; private set; }

        private void Deserialize()
        {
            XDocument enemyDefinitionsDoc = XDocument.Parse(_enemyDefinitions.text, LoadOptions.SetBaseUri);
            EnemyDefinitions = new System.Collections.Generic.Dictionary<string, EnemyDefinition>();

            XElement enemyDefinitionsElement = enemyDefinitionsDoc.Element("FakeEnemyDefinitions");
            foreach (XElement enemyDefinitionElement in enemyDefinitionsElement.Elements("EnemyDefinition"))
            {
                EnemyDefinition enemyDefinition = new EnemyDefinition(enemyDefinitionElement);
                EnemyDefinitions.Add(enemyDefinition.Id, enemyDefinition);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Deserialize();
        }
    }
}