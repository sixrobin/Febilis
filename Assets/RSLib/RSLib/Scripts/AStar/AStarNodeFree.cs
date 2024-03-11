namespace RSLib.AStar
{
	using UnityEngine;

	/// <summary>
	/// Node belonging to any A* free mesh.
	/// </summary>
	public class AStarNodeFree : AStarNode
    {
		public AStarNodeFree(Vector3 worldPos, int baseCost) : base(worldPos, baseCost)
		{

		}

		public override void OnNodeRemoved(AStarNode node)
		{
			if (node == this)
				return;

			if (Neighbours.Contains(node))
				Neighbours.Remove(node);
		}

		/// <summary>
		/// Compares both nodes using their world positions, adding their base cost.
		/// </summary>
		/// <param name="node">The compared node.</param>
		/// <returns>The cost to move to the other node.</returns>
		public override int CostToNode(AStarNode node)
        {
			return (int)(WorldPos - node.WorldPos).sqrMagnitude + BaseCost * BaseCost;
        }

		/// <summary>
		/// Removes the node from the mesh it belongs to.
		/// </summary>
		public void RemoveFromMesh()
		{
			Mesh.RemoveNode(this);
		}
	}
}