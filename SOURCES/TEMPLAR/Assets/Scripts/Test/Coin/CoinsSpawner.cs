namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CoinsSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _coinPrefab = null;

        private void SpawnCoins(Vector3 pos, int count)
        {
            for (int i = 0; i < count; ++i)
                Instantiate(_coinPrefab, pos, _coinPrefab.transform.rotation);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
                SpawnCoins(UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition).WithZ(0f), Random.Range(3, 5));
        }
    }
}