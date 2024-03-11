namespace RSLib.AStar
{
	using System.Collections.Generic;
	using UnityEngine;
	using Extensions;

	/// <summary>
	/// Class used to place A* free mesh nodes in the scene.
	/// </summary>
	public class AStarNodeFreeGO : MonoBehaviour
	{
		public AStarNodeFree Node { get; private set; }

		[Header ("A* SETTINGS")]
		[SerializeField] private AStarNodeFreeGO[] _neighbours = null;
		[SerializeField] private int _baseCost = 1;
		[SerializeField] private bool _availableOnAwake = true;

#if UNITY_EDITOR
        [Header("EDITOR")]
        [SerializeField] private float _autoFindRadius = 6;
        [SerializeField] private float _costHeight = 0.5f;
#endif

        /// <summary>
        /// Adds the node to a given mesh, setting up its neighbours.
        /// </summary>
        /// <param name="mesh">The mesh to add this node to.</param>
        public void AddToMesh(AStarMeshFree mesh)
		{
            List<AStarNode> neighboursNodes = new List<AStarNode>();
			foreach (AStarNodeFreeGO neighbourGO in _neighbours)
				neighboursNodes.Add(neighbourGO.Node);

			mesh.AddNode(Node, neighboursNodes);
		}

		private void Awake()
		{
			Node = new AStarNodeFree(transform.position, _baseCost) { IsAvailable = _availableOnAwake };
		}

        private void Start()
		{
			List<AStarNode> neighbours = new List<AStarNode>();
			foreach (AStarNodeFreeGO neighbourGO in _neighbours)
				neighbours.Add (neighbourGO.Node);

			Node.Neighbours = neighbours;
		}

#if UNITY_EDITOR
		[ContextMenu("Find neighbours")]
		private void FindNeighboursAutomatically()
		{
			List<AStarNodeFreeGO> closeNodes = new List<AStarNodeFreeGO>();
			foreach (AStarNodeFreeGO node in FindObjectsOfType<AStarNodeFreeGO>())
				if (node != this && (node.transform.position - transform.position).sqrMagnitude < _autoFindRadius * _autoFindRadius)
					closeNodes.Add(node);

			_neighbours = closeNodes.ToArray();
			Debug.Log ($"Found {_neighbours.Length} neighbours in a radius of {_autoFindRadius}.", gameObject);
		}

        private void OnDrawGizmos()
        {
            Gizmos.color = Node == null || !Node.IsAvailable ? Color.red : Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.4f);

            if (UnityEditor.EditorApplication.isPlaying)
            {
	            UnityEngine.Assertions.Assert.IsNotNull(Node, "Trying to draw gizmos with a null Node.");
	            
                if (Node.Neighbours == null || Node.Neighbours.Count == 0)
                    return;

                foreach (AStarNode neighbour in Node.Neighbours)
                {
                    Gizmos.color = (!neighbour.Neighbours.Contains(Node)
                        ? Color.white
                        : !neighbour.IsAvailable || !Node.IsAvailable
                            ? Color.red
                            : Color.cyan).WithA(0.25f);

                    Gizmos.DrawLine(transform.position + ((neighbour as AStarNodeFree).WorldPos - transform.position).NormalNormalized(VectorExtensions.Axis.XY, true) * 0.05f,
                        (neighbour as AStarNodeFree).WorldPos - (transform.position - (neighbour as AStarNodeFree).WorldPos).NormalNormalized(VectorExtensions.Axis.XY, true) * 0.05f);
                }
            }
            else
            {
                if (_neighbours == null || _neighbours.Length == 0)
                    return;

                foreach (AStarNodeFreeGO neighbour in _neighbours)
                {
                    Gizmos.color = Color.white.WithA(0.15f);
                    Gizmos.DrawLine(transform.position + (neighbour.transform.position - transform.position).NormalNormalized(VectorExtensions.Axis.XY, true) * 0.05f,
                        neighbour.transform.position - (transform.position - neighbour.transform.position).NormalNormalized(VectorExtensions.Axis.XY, true) * 0.05f);
                }
            }

            if (Node != null && Node.IsAvailable)
                UnityEditor.Handles.Label(transform.position + Vector3.up * _costHeight, $"COST: {Node.BaseCost}", UnityEditor.EditorStyles.boldLabel);
		}
#endif
	}
}