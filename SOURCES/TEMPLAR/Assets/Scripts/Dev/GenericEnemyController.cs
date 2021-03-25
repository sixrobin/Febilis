namespace Templar.Dev
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class GenericEnemyController : MonoBehaviour
    {
        [SerializeField] private string _id = string.Empty;

        public RSLib.HealthSystem HealthSystem { get; private set; }

        private void Awake()
        {
            
        }
    }
}