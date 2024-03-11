namespace RSLib.AStar
{
    using UnityEngine;

    public class AStarNodeGrid : AStarNode
    {
        public int GridX { get; }
        public int GridY { get; }

        public AStarNodeGrid(int gridX, int gridY, Vector3 worldPos, int baseCost) : base(worldPos, baseCost)
        {
            GridX = gridX;
            GridY = gridY;
        }

        public override int CostToNode(AStarNode node)
        {
            int xDist = Mathf.Abs((node as AStarNodeGrid).GridX - GridX);
            int yDist = Mathf.Abs((node as AStarNodeGrid).GridY - GridY);

            return 10 * BaseCost +
                xDist > yDist 
                    ? 14 * yDist + 10 * (xDist - yDist) 
                    : 14 * xDist + 10 * (yDist - xDist);
        }
    }
}