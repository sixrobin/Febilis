namespace Templar.Physics.Destroyables
{
    using UnityEngine;

    public class Crate : MonoBehaviour, IDestroyableObject
    {
        public DestroyableSourceType ValidSourcesTypes => DestroyableSourceType.ALL;

        public void Destroy(DestroyableSourceType sourceType)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                ValidSourcesTypes.HasFlag(sourceType),
                $"Destroying {transform.name} from a source of type {sourceType.ToString()} even though it is not a valid source.");

            Debug.Log($"{transform.name} destroyed from a source of type {sourceType.ToString()}.");
            gameObject.SetActive(false);
        }
    }
}