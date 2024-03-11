namespace Templar.Datas.Unit.Enemy
{
    using System.Xml.Linq;

    public class PlayerDetectedEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "PlayerDetected";

        public PlayerDetectedEnemyConditionDatas(XContainer container) : base(container)
        {
        }
    }
}