namespace Templar.Datas.Unit.Enemy
{
    using System.Xml.Linq;

    public class PlayerAboveEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "PlayerAbove";

        public PlayerAboveEnemyConditionDatas(XContainer container) : base(container)
        {
        }
    }
}