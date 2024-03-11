namespace RSLib.AStar
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AgentFreeMesh : MonoBehaviour
    {
        [SerializeField] private AStarNodeFreeGO _start = null;
        [SerializeField] private AStarNodeFreeGO _destination = null;

        private List<AStarNode> _path = new List<AStarNode>();

        private IEnumerator FollowPathCoroutine()
        {
            yield return new WaitForSeconds(0.1f);

            while (true)
            {
                _path = AStar.FindPath(_start.Node, _destination.Node);
                transform.position = _path[0].WorldPos;

                for (int i = 1; i < _path.Count; ++i)
                {
                    while (Vector3.Distance(transform.position, _path[i].WorldPos) > 0.05f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, _path[i].WorldPos, Time.deltaTime * 8f);
                        yield return null;
                    }
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        private void Start()
        {
            StartCoroutine(FollowPathCoroutine());
        }

        private void OnDrawGizmos()
        {
            if (_path == null || _path.Count == 0)
                return;

            Gizmos.color = Color.yellow;
            for (int i = 1; i < _path.Count; ++i)
                Gizmos.DrawLine(_path[i - 1].WorldPos, _path[i].WorldPos);
        }
    }
}