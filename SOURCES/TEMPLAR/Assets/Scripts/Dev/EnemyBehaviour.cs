namespace Templar.Dev
{
    public class EnemyBehaviour : EnemyConditionsChecker
    {
        public EnemyBehaviour(GenericEnemyController enemyCtrl, EnemyBehaviourDatas behaviourDatas)
            : base(enemyCtrl, behaviourDatas)
        {
            BehaviourDatas = behaviourDatas;
            CreateActions();
        }

        public EnemyBehaviourDatas BehaviourDatas { get; private set; }

        public IEnemyAction[] Actions { get; private set; }

        private void CreateActions()
        {
            Actions = new IEnemyAction[BehaviourDatas.Actions.Count];

            for (int i = 0; i < Actions.Length; ++i)
            {
                if (BehaviourDatas.Actions[i] is AttackEnemyActionDatas attackDatas)
                    Actions[i] = new AttackEnemyAction(EnemyCtrl, attackDatas);
                else if (BehaviourDatas.Actions[i] is BackAndForthEnemyActionDatas backAndForthDatas)
                    Actions[i] = new BackAndForthEnemyAction(EnemyCtrl, backAndForthDatas);
                else if (BehaviourDatas.Actions[i] is FleeEnemyActionDatas fleeDatas)
                    Actions[i] = new FleeEnemyAction(EnemyCtrl, fleeDatas);
                else
                    CProLogger.LogError(this, $"Unknown action type {Actions[i].GetType().FullName}");
            }
        }
    }
}