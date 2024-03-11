namespace Templar.Datas.Unit.Enemy
{
    using System.Xml.Linq;

    public class FullHealthEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "FullHealth";

        public FullHealthEnemyConditionDatas(XContainer container) : base(container)
        {
        }
    }
}