namespace Templar.Database
{
    using System.Xml.Linq;
    using UnityEngine;

    public partial class AttackDatabase : RSLib.Framework.ConsoleProSingleton<AttackDatabase>, IDatabase
    {
        [SerializeField] private TextAsset[] _attacksDatas = null;
        [SerializeField] private AnimationCurve _defaultPlayerAttackCurve = RSLib.AnimationCurves.LinearReversed;

        public static System.Collections.Generic.Dictionary<string, Datas.Attack.PlayerAttackDatas> PlayerAttacksDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, Datas.Attack.EnemyAttackDatas> EnemyAttacksDatas { get; private set; }

        public static AnimationCurve DefaultPlayerAttackCurve => Instance._defaultPlayerAttackCurve;

        public void Load()
        {
            DeserializeAttacksDatas();
        }

        private void DeserializeAttacksDatas()
        {
            PlayerAttacksDatas = new System.Collections.Generic.Dictionary<string, Datas.Attack.PlayerAttackDatas>();
            EnemyAttacksDatas = new System.Collections.Generic.Dictionary<string, Datas.Attack.EnemyAttackDatas>();

            for (int i = _attacksDatas.Length - 1; i >= 0; --i)
            {
                XDocument attacksDatasDoc = XDocument.Parse(_attacksDatas[i].text, LoadOptions.SetBaseUri);
                XElement attacksDatasElement = attacksDatasDoc.Element("AttacksDatas");

                foreach (XElement playerAttackDatasElement in attacksDatasElement.Elements("PlayerAttack"))
                {
                    Datas.Attack.PlayerAttackDatas playerAttackDatas = new Datas.Attack.PlayerAttackDatas(playerAttackDatasElement);
                    PlayerAttacksDatas.Add(playerAttackDatas.Id, playerAttackDatas);
                }

                foreach (XElement enemyAttackDatasElement in attacksDatasElement.Elements("EnemyAttack"))
                {
                    Datas.Attack.EnemyAttackDatas enemyAttackDatas = new Datas.Attack.EnemyAttackDatas(enemyAttackDatasElement);
                    EnemyAttacksDatas.Add(enemyAttackDatas.Id, enemyAttackDatas);
                }
            }
            
            Log($"Deserialized {PlayerAttacksDatas.Count} player attacks datas and {EnemyAttacksDatas.Count} enemy attacks datas.");
        }
    }
}