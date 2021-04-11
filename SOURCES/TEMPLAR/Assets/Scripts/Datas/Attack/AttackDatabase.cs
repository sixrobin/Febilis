namespace Templar.Datas.Attack
{
    using System.Xml.Linq;
    using UnityEngine;

    public partial class AttackDatabase : RSLib.Framework.Singleton<AttackDatabase>
    {
        [SerializeField] private TextAsset _attacksDatas = null;
        [SerializeField] private AnimationCurve _defaultPlayerAttackCurve = RSLib.AnimationCurves.LinearReversed;

        public static System.Collections.Generic.Dictionary<string, PlayerAttackDatas> PlayerAttacksDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, EnemyAttackDatas> EnemyAttacksDatas { get; private set; }

        public static AnimationCurve DefaultPlayerAttackCurve => Instance._defaultPlayerAttackCurve;

        private void Deserialize()
        {
            XDocument attacksDatasDoc = XDocument.Parse(_attacksDatas.text, LoadOptions.SetBaseUri);

            PlayerAttacksDatas = new System.Collections.Generic.Dictionary<string, PlayerAttackDatas>();
            EnemyAttacksDatas = new System.Collections.Generic.Dictionary<string, EnemyAttackDatas>();

            XElement attacksDatasElement = attacksDatasDoc.Element("AttacksDatas");

            foreach (XElement playerAttackDatasElement in attacksDatasElement.Elements("PlayerAttack"))
            {
                PlayerAttackDatas playerAttackDatas = new PlayerAttackDatas(playerAttackDatasElement);
                PlayerAttacksDatas.Add(playerAttackDatas.Id, playerAttackDatas);
            }

            foreach (XElement enemyAttackDatasElement in attacksDatasElement.Elements("EnemyAttack"))
            {
                EnemyAttackDatas enemyAttackDatas = new EnemyAttackDatas(enemyAttackDatasElement);
                EnemyAttacksDatas.Add(enemyAttackDatas.Id, enemyAttackDatas);
            }

            Log($"Deserialized {PlayerAttacksDatas.Count} player attacks datas and {EnemyAttacksDatas.Count} enemy attacks datas.");
        }

        protected override void Awake()
        {
            base.Awake();
            Deserialize();
        }
    }

    public partial class AttackDatabase : RSLib.Framework.Singleton<AttackDatabase>
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