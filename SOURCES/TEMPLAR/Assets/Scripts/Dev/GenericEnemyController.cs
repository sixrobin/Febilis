namespace Templar.Dev
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class GenericEnemyController : Unit.UnitController
    {
        [Header("REFERENCES")]
        [SerializeField] private Transform _player = null;

        [Header("BEHAVIOUR")]
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private float _behaviourUpdateRate = 1f;

        [Header("DEBUG")]
        [SerializeField] private string _currBehaviourName = string.Empty;

        private float _behaviourUpdateTimer;

        private EnemyBehaviour _currBehaviour;
        public EnemyBehaviour CurrBehaviour
        {
            get => _currBehaviour;
            set
            {
                _currBehaviour = value;
                _currBehaviourName = _currBehaviour.BehaviourDatas.Name;
            }
        }

        public EnemyDatas EnemyDatas { get; private set; }
        public EnemyBehaviour[] Behaviours { get; private set; }

        public Transform Player => _player;

        private void UpdateCurrentBehaviour()
        {
            for (int i = 0; i < Behaviours.Length; ++i)
            {
                if (Behaviours[i].CheckConditions())
                {
                    CurrBehaviour = Behaviours[i];
                    return;
                }
            }

            CProLogger.LogError(this, $"No behaviour has validated its conditions for enemy {_id}.", gameObject);
        }

        private void Start()
        {
            UnityEngine.Assertions.Assert.IsTrue(EnemyDatabase.EnemiesDatas.ContainsKey(_id), $"Unknown enemy Id {_id}.");
            EnemyDatas = EnemyDatabase.EnemiesDatas[_id];

            Behaviours = new EnemyBehaviour[EnemyDatas.Behaviours.Count];
            for (int i = 0; i < Behaviours.Length; ++i)
                Behaviours[i] = new EnemyBehaviour(this, EnemyDatas.Behaviours[i]);

            UpdateCurrentBehaviour();
        }

        private void Update()
        {
            _behaviourUpdateTimer += Time.deltaTime;
            if (_behaviourUpdateTimer > _behaviourUpdateRate)
            {
                _behaviourUpdateTimer = 0f;
                UpdateCurrentBehaviour();
            }
        }
    }
}