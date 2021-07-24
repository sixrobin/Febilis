namespace Templar.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Shake Settings Datas", menuName = "Datas/Shake/Shake Settings")]
    public class ShakeSettingsDatas : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _radius = 0.3f;
        [SerializeField, Min(0f)] private float _speed = 2f;

        public float Radius => _radius;
        public float Speed => _speed;

        public static ShakeSettingsDatas Default
        {
            get
            {
                ShakeSettingsDatas shakeSettingsDatas = CreateInstance<ShakeSettingsDatas>();
                shakeSettingsDatas._radius = 0.3f;
                shakeSettingsDatas._speed = 2f;

                return shakeSettingsDatas;
            }
        }
    }
}