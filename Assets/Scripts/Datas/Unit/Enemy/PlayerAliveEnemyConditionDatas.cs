namespace Templar.Datas.Unit.Enemy
{
    using System.Xml.Linq;

    public class PlayerAliveEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "PlayerAlive";

        public PlayerAliveEnemyConditionDatas(XContainer container) : base(container)
        {
        }
    }
}