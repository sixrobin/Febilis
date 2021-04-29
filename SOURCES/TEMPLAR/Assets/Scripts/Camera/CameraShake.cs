namespace Templar.Camera
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CameraShake
    {
        public const string ID_SMALL = "Small";
        public const string ID_MEDIUM = "Medium";
        public const string ID_BIG = "Big";

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

        private Settings _shakeSettings;

        public Vector2 Trauma { get; private set; }

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
            Trauma = (Trauma + Vector2.one * value).ClampAll01();
        }

        public void AddTrauma(Vector2 value)
        {
            Trauma = (Trauma + value).ClampAll01();
        }

        public void AddTrauma(float x, float y)
        {
            Trauma = (Trauma + new Vector2(x, y)).ClampAll01();
        }

        public void SetTrauma(float value)
        {
            Trauma = Vector2.one * Mathf.Clamp01(value);
        }

        public void SetTrauma(Vector2 value)
        {
            Trauma = value.ClampAll01();
        }

        public void SetTrauma(float x, float y)
        {
            Trauma = new Vector2(Mathf.Clamp01(x), Mathf.Clamp01(y));
        }

        public void AddTraumaFromDatas(Templar.Datas.ShakeTraumaDatas datas)
        {
            if (datas == null)
                return;

            if (datas.AddType == Templar.Datas.ShakeTraumaDatas.ShakeAddType.ADDITIVE)
                AddTrauma(datas.X, datas.Y);
            else if (datas.AddType == Templar.Datas.ShakeTraumaDatas.ShakeAddType.OVERRIDE)
                SetTrauma(datas.X, datas.Y);
        }

        public void ApplyOnTransform(Transform transform)
        {
            transform.position += GetShake();
        }

        public Vector3 GetShake()
        {
            if (Trauma.sqrMagnitude == 0f || Manager.FreezeFrameManager.IsFroze)
                return Vector3.zero;

            Trauma = Trauma.AddAll(-_shakeSettings.Speed * Time.deltaTime);
            Trauma = Trauma.ClampAll01();

            Vector2 rndDir = Random.insideUnitCircle.normalized;
            rndDir *= Trauma;

            return (Vector3)(rndDir * _shakeSettings.Radius);
        }
    }
}