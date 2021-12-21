namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;
    using System.Linq;

    public class Test : MonoBehaviour
    {
        public RSLib.Framework.Collections.FixedSizedConcurrentQueue<int> queue;

        private void Start()
        {
            queue = new RSLib.Framework.Collections.FixedSizedConcurrentQueue<int>(3);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                queue.Enqueue(Random.Range(0, 10));
                Debug.Log(string.Join(",", queue.Select(o => o.ToString())));
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                queue.TryPeek(out _);
                Debug.Log(string.Join(",", queue.Select(o => o.ToString())));
            }
        }
    }
}