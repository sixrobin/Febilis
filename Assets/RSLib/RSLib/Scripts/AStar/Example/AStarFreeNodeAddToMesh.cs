namespace RSLib.AStar
{
    using UnityEngine;

    public class AStarFreeNodeAddToMesh : MonoBehaviour
    {
        [Header("PRESS A TO ADD")]
        [SerializeField] private AStarNodeFreeGO _freeNode = null;
        [SerializeField] private AStarMeshFree _mesh = null;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
                _freeNode.AddToMesh(_mesh);
        }
    }
}