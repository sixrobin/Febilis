namespace Templar.Datas.Unit.Enemy
{
    using System.Xml.Linq;
    using UnityEngine;

    public partial class EnemyDatabase : RSLib.Framework.Singleton<EnemyDatabase>
    {
        [SerializeField] private TextAsset _enemiesDatas = null;

        public static System.Collections.Generic.Dictionary<string, EnemyDatas> EnemiesDatas { get; private set; }

        private void Deserialize()
        {
            XDocument enemiesDatasDoc = XDocument.Parse(_enemiesDatas.text, LoadOptions.SetBaseUri);
            EnemiesDatas = new System.Collections.Generic.Dictionary<string, EnemyDatas>();

            XElement enemiesDatasElement = enemiesDatasDoc.Element("EnemiesDatas");
            foreach (XElement enemyDatasElement in enemiesDatasElement.Elements("EnemyDatas"))
            {
                EnemyDatas enemyDatas = new EnemyDatas(enemyDatasElement);
                EnemiesDatas.Add(enemyDatas.Id, enemyDatas);
            }

            Log($"Deserialized {EnemiesDatas.Count} enemies datas.");
        }

        protected override void Awake()
        {
            base.Awake();
            Deserialize();
        }
    }

    public partial class EnemyDatabase : RSLib.Framework.Singleton<EnemyDatabase>
    {
        public override void Log(string msg)
        {
            CProLogger.Log(this, msg, gameObject);
        }

        public override void Log(string msg, Object context)
        {
            CProLogger.Log(this, msg, gameObject);
        }

        public override void LogError(string msg)
        {
            CProLogger.LogError(this, msg, gameObject);
        }

        public override void LogError(string msg, Object context)
        {
            CProLogger.LogError(this, msg, gameObject);
        }
    }
}