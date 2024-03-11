namespace RSLib.AStar
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Extensions;

    public class AgentGridMesh : MonoBehaviour
    {
        [SerializeField] private AStarMeshGrid _grid = null;
        [SerializeField] private Transform _destination = null;

        private List<AStarNode> _path = new List<AStarNode>();
        private AStarNode _startNode;
        private AStarNode _destinationNode;

        private IEnumerator FollowPathCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            _startNode = _grid.NodeFromWorldPos(transform.position);
            _destinationNode = _grid.NodeFromWorldPos(_destination.position);

            while (true)
            {
                _path = AStar.FindPath(_startNode, _destinationNode);
                transform.position = _path[0].WorldPos;

                for (int i = 1; i < _path.Count; ++i)
                {
                    while (Vector3.Distance(transform.position, _path[i].WorldPos) > 0.05f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, _path[i].WorldPos, Time.deltaTime * 6f);
                        yield return null;
                    }

                    transform.position = _path[i].WorldPos;
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
                Gizmos.DrawLine(_path[i - 1].WorldPos.WithY(.05f), _path[i].WorldPos.WithY(.05f));
        }
    }
}