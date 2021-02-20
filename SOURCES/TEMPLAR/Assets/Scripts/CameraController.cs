using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CameraShake.Settings _shakeSettings = CameraShake.Settings.Default;

    public CameraShake Shake { get; private set; }

    private void Awake()
    {
        Shake = new CameraShake(transform, _shakeSettings);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            Shake.SetTrauma(1f, 1f);
    }

    private void LateUpdate()
    {
        Shake.Apply();
    }

    private void OnValidate()
    {
        Shake?.SetSettings(_shakeSettings);
    }
}