namespace Templar.Unit.Enemy.Conditions
{
    public class MaxHeightOffsetEnemyCondition : EnemyCondition<Datas.Unit.Enemy.MaxHeightOffsetConditionDatas>
    {
        public MaxHeightOffsetEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.MaxHeightOffsetConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(UnityEngine.Mathf.Abs(EnemyCtrl.transform.position.y - EnemyCtrl.PlayerCtrl.transform.position.y) < ConditionsDatas.Offset);
        }
    }
}