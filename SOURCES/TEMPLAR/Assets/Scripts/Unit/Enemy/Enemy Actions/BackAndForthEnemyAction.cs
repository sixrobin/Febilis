namespace Templar.Unit.Enemy.Actions
{
    public class BackAndForthEnemyAction : EnemyAction<Datas.Unit.Enemy.BackAndForthEnemyActionDatas>
    {
        private float _pauseDir;
        private float _pauseTimer;
        private bool _sideCollision;

        public BackAndForthEnemyAction(EnemyController enemyCtrl, Datas.Unit.Enemy.BackAndForthEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
        }

        public float InitX { get; private set; }

        public bool IsOnPause { get; private set; }
        public float NextPauseDur { get; private set; }

        public float CurrRange { get; private set; }
        public float HalfCurrRange => CurrRange * 0.5f;

        public override void Init()
        {
            base.Init();

            EnemyCtrl.CollisionsCtrl.CollisionDetected += OnCollisionDetected;
            InitX = EnemyCtrl.transform.position.x;
            CurrRange = ActionDatas.Range;
        }

        public override bool CanExit()
        {
            return true;
        }

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

            if (reachedLimit || _sideCollision)
            {
                _sideCollision = false;

                _pauseDir = EnemyCtrl.CurrDir;
                EnemyCtrl.SetDirection(-EnemyCtrl.CurrDir);

                IsOnPause = true;
                ComputeNextPauseDuration();
                ComputeRangeWithFluctuation();

                EnemyCtrl.EnemyView.PlayWalkAnimation(false);

                return;
            }

            EnemyCtrl.Translate(EnemyCtrl.CurrDir * EnemyCtrl.EnemyDatas.WalkSpeed, 0f, checkEdge: true);
            EnemyCtrl.EnemyView.FlipX(EnemyCtrl.CurrDir < 0f);
            EnemyCtrl.EnemyView.PlayWalkAnimation(true);
        }

        public override void Reset()
        {
            base.Reset();
            IsOnPause = false;
            ComputeNextPauseDuration();
            ComputeRangeWithFluctuation();
        }

        private void OnCollisionDetected(Physics.CollisionsController.CollisionInfos collisionInfos)
        {
            if (EnemyCtrl.CurrAction != this)
                return;

            _sideCollision = EnemyCtrl.CurrDir == 1f && collisionInfos.Origin == Physics.CollisionsController.CollisionOrigin.RIGHT
                || EnemyCtrl.CurrDir == -1f && collisionInfos.Origin == Physics.CollisionsController.CollisionOrigin.LEFT
                || collisionInfos.Origin == Physics.CollisionsController.CollisionOrigin.EDGE;

            //if (_sideCollision)
            //    CProLogger.Log(EnemyCtrl, collisionInfos.Origin.ToString(), EnemyCtrl.gameObject);
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