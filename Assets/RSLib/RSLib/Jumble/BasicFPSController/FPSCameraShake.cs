namespace RSLib.Jumble.FPSController
{
    using UnityEngine;

    /// <summary>
    /// Extra movement to apply to the FPS camera to shake it.
    /// </summary>
    public class FPSCameraShake : FPSCameraExtraMovement
    {
        [SerializeField] private Shake.ShakeSettings _settings = Shake.ShakeSettings.Default;

        public static FPSCameraShake Instance;
        private Transform _transform;
        private Shake _shake;

        public override void ApplyMovement()
        {
            ApplyShake();
        }

        private void ApplyShake()
        {
            (Vector3 pos, Quaternion rot)? shake = _shake.Evaluate(transform);
            if (shake == null)
                return;

            _transform.position += shake.Value.pos;
            _transform.rotation *= shake.Value.rot;
        }

        public void AddTrauma(float value)
        {
            _shake.AddTrauma(value);
        }

        public void SetTrauma(float value)
        {
            _shake.SetTrauma(value);
        }

        public void ResetTrauma()
        {
            _shake.SetTrauma(0f);
        }

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
            _transform = transform;
            _shake = new Shake(_settings);
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}