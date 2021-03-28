namespace Templar.Dev
{
    public class BackAndForthEnemyAction : EnemyAction<BackAndForthEnemyActionDatas>
    {
        private float _pauseDir;
        private float _pauseTimer;

        public BackAndForthEnemyAction(GenericEnemyController enemyCtrl, BackAndForthEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
            InitX = enemyCtrl.transform.position.x;
            CurrRange = actionDatas.Range;
        }

        public float InitX { get; private set; }

        public bool IsOnPause { get; private set; }
        public float NextPauseDur { get; private set; }

        public float CurrRange { get; private set; }
        public float HalfCurrRange => CurrRange * 0.5f;

        public override void Execute()
        {
            if (EnemyCtrl.BeingHurt)
                return;

            if (IsOnPause)
            {
                _pauseTimer += UnityEngine.Time.deltaTime;
                EnemyCtrl.EnemyView.FlipX(_pauseDir < 0f);

                if (_pauseTimer < NextPauseDur)
                    return;

                _pauseTimer = 0f;
                IsOnPause = false;
            }

            bool reachedLimit = EnemyCtrl.CurrDir == 1f
                ? EnemyCtrl.transform.position.x >= InitX + HalfCurrRange
                : EnemyCtrl.transform.position.x <= InitX - HalfCurrRange;

            if (reachedLimit)
            {
                _pauseDir = EnemyCtrl.CurrDir;
                EnemyCtrl.SetDirection(-EnemyCtrl.CurrDir);

                IsOnPause = true;
                ComputeNextPauseDuration();
                ComputeRangeWithFluctuation();
                return;
            }

            EnemyCtrl.Translate(EnemyCtrl.CurrDir * EnemyCtrl.EnemyDatas.WalkSpeed, 0f);
            EnemyCtrl.EnemyView.FlipX(EnemyCtrl.CurrDir < 0f);
        }

        public override void Reset()
        {
            base.Reset();
            IsOnPause = false;
            ComputeNextPauseDuration();
            ComputeRangeWithFluctuation();
        }

        private void ComputeNextPauseDuration()
        {
            NextPauseDur = UnityEngine.Random.Range(ActionDatas.PauseDur.min, ActionDatas.PauseDur.max);
        }

        private void ComputeRangeWithFluctuation()
        {
            if (ActionDatas.RangeFluctuationOnPause > 0f)
                CurrRange = ActionDatas.Range + UnityEngine.Random.Range(-ActionDatas.RangeFluctuationOnPause, ActionDatas.RangeFluctuationOnPause);
        }
    }
}