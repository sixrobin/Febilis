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

            public RecoilSettings(float force, float dur)
            {
                _force = force;
                _dur = dur;
            }

            public float Force => _force;
            public float Dur => _dur;
        }

        private RecoilSettings _settings;

        public Recoil(RecoilSettings template, float dir)
        {
            _settings = new RecoilSettings(template.Force, template.Dur);
            Dur = template.Dur;
            Dir = dir;
        }

        public Recoil(float dir, float force, float dur)
        {
            _settings = new RecoilSettings(force, dur);
            Dur = _settings.Dur;
            Dir = dir;
        }

        public float Force => _settings.Force;
        public float Dir { get; private set; }
        public float Dur { get; private set; }
        public bool IsComplete => Dur <= 0f;

        public void Update()
        {
            Dur -= Time.deltaTime;
        }
    }
}