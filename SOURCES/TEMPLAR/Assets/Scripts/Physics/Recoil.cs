namespace Templar.Physics
{
    using UnityEngine;

    public class Recoil
    {
        [System.Serializable]
        public class RecoilSettings
        {
            [SerializeField, Min(0f)] private float _force = 5f;
            [SerializeField, Min(0f)] private float _dur = 0.5f;
            [SerializeField, Min(1f)] private float _airborneMult = 1f;
            [SerializeField] private bool _checkEdge = false;

            public RecoilSettings(float force, float dur, float airborneMult)
            {
                _force = force;
                _dur = dur;
                _airborneMult = airborneMult;
            }

            public float Force => _force;
            public float Dur => _dur;
            public float AirborneMult => _airborneMult;
            public bool CheckEdge => _checkEdge;
        }

        private RecoilSettings _settings;

        public Recoil(RecoilSettings template, float dir)
        {
            _settings = new RecoilSettings(template.Force, template.Dur, template.AirborneMult);
            Dur = template.Dur;
            Dir = dir;
            CheckEdge = template.CheckEdge;
        }

        public Recoil(float dir, float force, float dur, float airborneMult, bool checkEdge = false)
        {
            _settings = new RecoilSettings(force, dur, airborneMult);
            Dur = _settings.Dur;
            Dir = dir;
            CheckEdge = checkEdge;
        }

        public float Force => _settings.Force;
        public float AirborneMult => _settings.AirborneMult;
        public float Dir { get; private set; }
        public float Dur { get; private set; }
        public bool CheckEdge { get; private set; }
        public bool IsComplete => Dur <= 0f;
        
        public void Update()
        {
            Dur -= Time.deltaTime;
        }
    }
}