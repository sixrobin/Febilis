namespace RSLib
{
    using UnityEngine;

    /// <summary>
    /// ParticlesSpawner class override that uses RSLib pool, thus requiring pool files.
    /// </summary>
    [System.Serializable]
    public class ParticlesSpawnerPool : ParticlesSpawner
    {
        protected override GameObject SpawnParticle(Particle particle, Transform parent)
        {
            GameObject instance = RSLib.Framework.Pooling.Pool.Get(particle.Prefab);
            instance.transform.SetParent(parent);
            return instance;
        }
    }
}