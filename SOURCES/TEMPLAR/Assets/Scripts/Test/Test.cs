namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;
    using System.Linq;

    public class Test : MonoBehaviour
    {
        public RSLib.ParticlesSpawnerPool _particlesSpawner;
        
        private void Start()
        {
            _particlesSpawner.SpawnParticles(transform.position);
        }

        private void Update()
        {
        }
    }
}