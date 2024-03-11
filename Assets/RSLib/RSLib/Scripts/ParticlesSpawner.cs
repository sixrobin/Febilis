namespace RSLib
{
    using UnityEngine;

    /// <summary>
    /// Used to spawn multiple particles (or actually any prefab) from a position, with the ability to add
    /// a global offset and an individual offset for each particle.
    /// Could be used for particles such as blood burst, footstep puffs, etc.
    /// </summary>
    [System.Serializable]
    public class ParticlesSpawner
    {
        [System.Serializable]
        protected struct Particle
        {
            public GameObject Prefab;
            public Vector3 Offset;
        }

        [SerializeField] private Vector3 _globalOffset = Vector3.zero;
        [SerializeField] private Particle[] _particles = null;

        public void SpawnParticles(Vector3 position, Transform parent = null)
        {
            for (int i = 0; i < _particles.Length; ++i)
            {
                Particle particle = _particles[i];

                if (particle.Prefab == null)
                {
                    UnityEngine.Debug.LogWarning($"Particle to spawn is null at index {i}.");
                    continue;
                }
                
                GameObject particleInstance = SpawnParticle(particle, parent);
                particleInstance.transform.position = position + particle.Offset + _globalOffset;
            }
        }

        protected virtual GameObject SpawnParticle(Particle particle, Transform parent)
        {
            return Object.Instantiate(particle.Prefab, parent);
        }
    }
}