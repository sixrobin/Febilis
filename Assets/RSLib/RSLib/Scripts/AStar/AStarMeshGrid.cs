namespace RSLib.AStar
{
    using UnityEngine;
    using Extensions;

    /// <summary>
    /// Grid mesh placed anywhere in the scene, based on grid size and nodes sizes.
    /// Holds a 2D array of AStarNodeGrid which dimensions are not editable.
    /// </summary>
    public class AStarMeshGrid : AStarMesh
    {
        [Header("GRID SETTINGS")]
        [SerializeField] private LayerMask UnwalkableMask = 0;
        [SerializeField] private Vector2 Dimensions = Vector2.zero;
        [SerializeField] private float NodeRadius = 1;
        [SerializeField] private bool _diagonalNeighbours = true;

        private AStarNodeGrid[,] _mesh;
        private int _width;
        private int _height;

        public override int Size => _width * _height;

        private float NodeDiameter => NodeRadius * 2f;

        public override void ResetNodes()
        {
            foreach (AStarNodeGrid node in _mesh)
                node.Reset();
        }

        public override bool ContainsNode(AStarNode node)
        {
            for (int x = 0; x < _width; ++x)
                for (int y = 0; y < _height; ++y)
                    if (_mesh[x, y] == node)
                        return true;

            return false;
        }

        /// <summary>
        /// Retrieves the closest node to any world position (clamped inside the grid).
        /// WARNING: Takes into account this object transform but not its potential parents.
        /// </summary>
        /// <param name="worldPos">The position to find a node from.</param>
        /// <returns>The closest node.</returns>
        public AStarNodeGrid NodeFromWorldPos(Vector3 worldPos)
        {
            worldPos -= transform.position;

            float xPercent = Mathf.Clamp01((worldPos.x + Dimensions.x * 0.5f) / Dimensions.x);
            float yPercent = Mathf.Clamp01((worldPos.z + Dimensions.y * 0.5f) / Dimensions.y);

            int x = Mathf.RoundToInt((_width - 1) * xPercent);
            int y = Mathf.RoundToInt((_height - 1) * yPercent);
            return _mesh[x, y];
        }

        /// <summary>
        /// Re-bakes the mesh. Can be used if obstacles positions did change.
        /// </summary>
        public void Refresh()
        {
            Bake();
        }

        /// <summary>
        /// Generates nodes inside the grid size, depending on the nodes sizes.
        /// Checks for potential obstacles using the non-walkable mask, and sets nodes on obstacles as unavailable.
        /// </summary>
        protected override void Bake()
        {
            _width = Mathf.RoundToInt(Dimensions.x / NodeDiameter);
            _height = Mathf.RoundToInt(Dimensions.y / NodeDiameter);

            _mesh = new AStarNodeGrid[_width, _height];
            Vector3 worldBottomLeft = transform.position - Vector3.right * Dimensions.x * 0.5f - Vector3.forward * Dimensions.y * 0.5f;

            for (int x = 0; x < _width; ++x)
            {
                for (int y = 0; y < _height; ++y)
                {
                    Vector3 worldPos = worldBottomLeft + Vector3.right * (x * NodeDiameter + NodeRadius) + Vector3.forward * (y * NodeDiameter + NodeRadius);
                    bool walkable = !Physics.CheckSphere(worldPos, NodeRadius, UnwalkableMask);
                    _mesh[x, y] = new AStarNodeGrid(x, y, worldPos, 1) { IsAvailable = walkable };
                    _mesh[x, y].SetMesh(this);
                }
            }

            for (int x = 0; x < _width; ++x)
                for (int y = 0; y < _height; ++y)
                    _mesh[x, y].Neighbours = GetNodeNeighbours(x, y);
        }

        /// <summary>
        /// Gets any node its neighbours, using the 2D arrays and depending on the diagonal move allowing.
        /// </summary>
        /// <param name="nodeX">X index of the node.</param>
        /// <param name="nodeY">Y index of the node.</param>
        /// <returns>List of the node neighbours.</returns>
        private System.Collections.Generic.List<AStarNode> GetNodeNeighbours(int nodeX, int nodeY)
        {
            System.Collections.Generic.List<AStarNode> neighbours = new System.Collections.Generic.List<AStarNode>();

            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (!_diagonalNeighbours && Mathf.Abs(x) + Mathf.Abs(y) == 2)
                        continue;

                    if (nodeX + x < 0 || nodeX + x >= _width || nodeY + y < 0 || nodeY + y >= _height)
                        continue;

                    neighbours.Add(_mesh[nodeX + x, nodeY + y]);
                }
            }

            return neighbours;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white.WithA(0.5f);
            Gizmos.DrawWireCube(transform.position, new Vector3(Dimensions.x, 1, Dimensions.y));

            if (_mesh == null || _mesh.GetLength(0) == 0 || _mesh.GetLength(1) == 0)
                return;

            foreach (AStarNodeGrid node in _mesh)
            {
                Gizmos.color = node.IsAvailable ? Color.cyan : Color.red;
                Gizmos.DrawSphere(node.WorldPos, 0.12f);

                Gizmos.color = Color.white.WithA(0.1f);
                foreach (AStarNode neighbour in node.Neighbours)
                    Gizmos.DrawLine(node.WorldPos, neighbour.WorldPos);
            }
        }
#endif
    }
}