namespace Templar.Datas.Attack
{
    using System.Xml.Linq;
    using UnityEngine;

    public partial class AttackDatabase : RSLib.Framework.ConsoleProSingleton<AttackDatabase>
    {
        [SerializeField] private TextAsset[] _attacksDatas = null;
        [SerializeField] private AnimationCurve _defaultPlayerAttackCurve = RSLib.AnimationCurves.LinearReversed;

        public static System.Collections.Generic.Dictionary<string, PlayerAttackDatas> PlayerAttacksDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, EnemyAttackDatas> EnemyAttacksDatas { get; private set; }

        public static AnimationCurve DefaultPlayerAttackCurve => Instance._defaultPlayerAttackCurve;

        private void Deserialize()
        {
            PlayerAttacksDatas = new System.Collections.Generic.Dictionary<string, PlayerAttackDatas>();
            EnemyAttacksDatas = new System.Collections.Generic.Dictionary<string, EnemyAttackDatas>();

            for (int i = _attacksDatas.Length - 1; i >= 0; --i)
            {
                XDocument attacksDatasDoc = XDocument.Parse(_attacksDatas[i].text, LoadOptions.SetBaseUri);
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
            }
            
            Log($"Deserialized {PlayerAttacksDatas.Count} player attacks datas and {EnemyAttacksDatas.Count} enemy attacks datas.");
        }

        protected override void Awake()
        {
            base.Awake();
            Deserialize();
        }
    }
}