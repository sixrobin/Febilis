namespace Templar.Unit.Enemy
{
    using Actions;

    public class EnemyBehaviour : EnemyConditionsChecker
    {
        public EnemyBehaviour(EnemyController enemyCtrl, Datas.Unit.Enemy.EnemyBehaviourDatas behaviourDatas)
            : base(enemyCtrl, behaviourDatas)
        {
            BehaviourDatas = behaviourDatas;
            CreateActions();
        }

        public Datas.Unit.Enemy.EnemyBehaviourDatas BehaviourDatas { get; private set; }

        public IEnemyAction[] Actions { get; private set; }

        private void CreateActions()
        {
            Actions = new IEnemyAction[BehaviourDatas.Actions.Count];

            for (int i = 0; i < Actions.Length; ++i)
            {
                if (BehaviourDatas.Actions[i] is Datas.Unit.Enemy.AttackEnemyActionDatas attackDatas)
                    Actions[i] = new AttackEnemyAction(EnemyCtrl, attackDatas);
                else if (BehaviourDatas.Actions[i] is Datas.Unit.Enemy.BackAndForthEnemyActionDatas backAndForthDatas)
                    Actions[i] = new BackAndForthEnemyAction(EnemyCtrl, backAndForthDatas);
                else if (BehaviourDatas.Actions[i] is Datas.Unit.Enemy.ChaseEnemyActionDatas chaseDatas)
                    Actions[i] = new ChaseEnemyAction(EnemyCtrl, chaseDatas);
                else if (BehaviourDatas.Actions[i] is Datas.Unit.Enemy.FleeEnemyActionDatas fleeDatas)
                    Actions[i] = new FleeEnemyAction(EnemyCtrl, fleeDatas);
                else
                    CProLogger.LogError(this, $"Unknown action type {Actions[i].GetType().FullName}");
            }
        }
    }
}