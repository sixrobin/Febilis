namespace RSLib.AStar
{
    using UnityEngine;

    /// <summary>
    /// Main class of any A* mesh.
    /// Contains event for adding or removing node, the size of the mesh and an abstract method to generate it.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AStarMesh : MonoBehaviour
    {
        /// <summary>
        /// Count of nodes inside the mesh.
        /// </summary>
        public abstract int Size { get; }

        public delegate void NodeChangeEventHandler(AStarNode node);

        public event NodeChangeEventHandler NodeAdded;
        public event NodeChangeEventHandler NodeRemoved;

        /// <summary>
        /// Resets the costs used to find a path. Should be called before the A* computation.
        /// </summary>
        public abstract void ResetNodes();

        /// <summary>
        /// Checks if a node is already in the mesh.
        /// </summary>
        public abstract bool ContainsNode(AStarNode node);

        public virtual void AddNode(AStarNode node)
        {
            NodeAdded?.Invoke(node);
        }

        public virtual void AddNode(AStarNode node, System.Collections.Generic.List<AStarNode> nodeNeighbours)
        {
            NodeAdded?.Invoke(node);
        }

        public virtual void RemoveNode(AStarNode node)
        {
            NodeRemoved?.Invoke(node);
        }

        /// <summary>
        /// Determines the way the mesh is constructed and how it is represented in code.
        /// </summary>
        protected abstract void Bake();

        protected virtual void Start()
        {
            Bake();
        }
    }
}