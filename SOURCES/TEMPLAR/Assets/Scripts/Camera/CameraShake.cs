namespace Templar.Camera
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CameraShake
    {
        [System.Serializable]
        public class Settings
        {
            [SerializeField] private float _radius = 0.3f;
            [SerializeField] private float _speed = 2f;

            public Settings(float radius, float speed)
            {
                _radius = radius;
                _speed = speed;
            }

            public Settings(Settings template)
            {
                _radius = template.Radius;
                _speed = template.Speed;
            }

            public static Settings Default => new Settings(0.3f, 2f);

            public float Radius => _radius;
            public float Speed => _speed;
        }

        private Vector2 _trauma = Vector2.zero;
        private Settings _shakeSettings;

        public CameraShake(Settings shakeSettings)
        {
            _shakeSettings = new Settings(shakeSettings);
        }

        public void SetSettings(float radius, float speed)
        {
            _shakeSettings = new Settings(radius, speed);
        }

        public void SetSettings(Settings template)
        {
            _shakeSettings = new Settings(template);
        }

        public void AddTrauma(float value)
        {
            _trauma = (_trauma + Vector2.one * value).ClampAll01();
        }

        public void AddTrauma(Vector2 value)
        {
            _trauma = (_trauma + value).ClampAll01();
        }

        public void AddTrauma(float x, float y)
        {
            _trauma = (_trauma + new Vector2(x, y)).ClampAll01();
        }

        public void SetTrauma(float value)
        {
            _trauma = Vector2.one * Mathf.Clamp01(value);
        }

        public void SetTrauma(Vector2 value)
        {
            _trauma = value.ClampAll01();
        }

        public void SetTrauma(float x, float y)
        {
            _trauma.x = Mathf.Clamp01(x);
            _trauma.y = Mathf.Clamp01(y);
        }

        public void ApplyOnTransform(Transform transform)
        {
            transform.position += GetShake();
        }

        public Vector3 GetShake()
        {
            if (_trauma.sqrMagnitude == 0f || Manager.FreezeFrameManager.IsFroze)
                return Vector3.zero;

            _trauma = _trauma.AddAll(-_shakeSettings.Speed * Time.deltaTime);
            _trauma = _trauma.ClampAll01();

            Vector2 rndDir = Random.insideUnitCircle.normalized;
            rndDir *= _trauma;

            return (Vector3)(rndDir * _shakeSettings.Radius);
        }
    }
}