namespace RSLib.AStar
{
	/// <summary>
	/// A* algorithm. Works with all AStarNode deriving classes.
	/// </summary>
	public static class AStar
	{
		private static Framework.Collections.Heap<AStarNode> _openSet;
		private static System.Collections.Generic.HashSet<AStarNode> _closeSet = new System.Collections.Generic.HashSet<AStarNode>();

		/// <summary>
		/// Compares the two nodes to make sure a path research can proceed.
		/// </summary>
		/// <param name="start">The starting node.</param>
		/// <param name="end">The destination node.</param>
		/// <returns>True if the algorithm is allowed to run, else false.</returns>
		private static bool CheckNodesValidity(AStarNode start, AStarNode end)
		{
			if (start == end)
			{
				UnityEngine.Debug.LogError("Starting node and destination node are the same.");
				return false;
			}

			if (start.Mesh == null)
			{
				UnityEngine.Debug.LogError("Starting node's A* mesh is null.");
				return false;
			}

			if (end.Mesh == null)
			{
				UnityEngine.Debug.LogError("Destination node's A* mesh is null.");
				return false;
			}

			if (start.Mesh != end.Mesh)
			{
				UnityEngine.Debug.LogError("Starting node and destination don't belong to the same A* mesh.");
				return false;
			}

			return true;
		}

		/// <summary>
		/// Returns the path to follow to go from a starting node to a destination node if both are in the same mesh.
		/// </summary>
		/// <param name="start">The starting node.</param>
		/// <param name="end">The destination node.</param>
		/// <returns>The path to follow.</returns>
		public static System.Collections.Generic.List<AStarNode> FindPath(AStarNode start, AStarNode end)
		{
			if (!CheckNodesValidity(start, end))
				return null;

			start.Mesh.ResetNodes();
			_openSet = new Framework.Collections.Heap<AStarNode>(start.Mesh.Size);
			_closeSet.Clear();

			_openSet.Add(start);

			while (_openSet.Count > 0)
			{
                AStarNode currentNode = _openSet.RemoveFirst();

				if (currentNode == end)
					return Retrace(start, end);

				_closeSet.Add(currentNode);

				foreach (AStarNode neighbour in currentNode.Neighbours)
				{
					if (!neighbour.IsAvailable || _closeSet.Contains(neighbour))
						continue;

					int neighbourCost = currentNode.GCost + neighbour.CostToNode(currentNode);

					if (neighbourCost < neighbour.GCost || !_openSet.Contains(neighbour))
					{
						neighbour.GCost = neighbourCost;
						neighbour.HCost = currentNode.GCost + neighbour.CostToNode(currentNode);
						neighbour.Parent = currentNode;

						if (!_openSet.Contains(neighbour))
							_openSet.Add(neighbour);
					}
				}
			}

			UnityEngine.Debug.LogError("No path found.");
			return null;
		}

		/// <summary>
		/// Rolls back the evaluated path once the A* algorithm is done processing.
		/// Starts loop from end node and traces back using each node parent.
		/// </summary>
		/// <param name="start">The starting node.</param>
		/// <param name="end">The destination node.</param>
		/// <returns>The retraced back path.</returns>
		private static System.Collections.Generic.List<AStarNode> Retrace(AStarNode start, AStarNode end)
		{
            System.Collections.Generic.List<AStarNode> path = new System.Collections.Generic.List<AStarNode>();
            AStarNode currentNode = end;

			while (currentNode != start)
			{
				path.Add(currentNode);
				currentNode = currentNode.Parent;
			}

			path.Add(start);
			path.Reverse();
			return path;
		}
	}
}