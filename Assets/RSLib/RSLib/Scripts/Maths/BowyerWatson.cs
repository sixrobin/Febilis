namespace RSLib.Maths
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Bowyer-Watson algorithm is a method for computing the Delaunay triangulation of a finite set of points.
    /// This implementation is Unity dependant and 2D only.
    /// https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
    /// </summary>
    public sealed class BowyerWatson
    {
        private const float SUPER_TRIANGLE_MARGIN = 5f;

        public readonly struct Triangle
        {
            public Triangle(Vector2 a, Vector2 b, Vector2 c)
            {
                // Compute vertices array.
                this.Vertices = new Vector2[]
                {
                    a,
                    b,
                    c
                };
                
                // Compute edges array.
                this.Edges = new Edge[]
                {
                    new Edge(a, b),
                    new Edge(b, c),
                    new Edge(c, a)
                };
                
                // Compute circumcenter.
                Vector2 sqrA = new Vector2(a.x * a.x, a.y * a.y);
                Vector2 sqrB = new Vector2(b.x * b.x, b.y * b.y);
                Vector2 sqrC = new Vector2(c.x * c.x, c.y * c.y);
                float d = (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) * 2f;
                float x = ((sqrA.x + sqrA.y) * (b.y - c.y) + (sqrB.x + sqrB.y) * (c.y - a.y) + (sqrC.x + sqrC.y) * (a.y - b.y)) / d;
                float y = ((sqrA.x + sqrA.y) * (c.x - b.x) + (sqrB.x + sqrB.y) * (a.x - c.x) + (sqrC.x + sqrC.y) * (b.x - a.x)) / d;
                this._circumcenter = new Vector2(x, y);

                // Compute circumradius (must be done AFTER circumcenter).
                this._circumradius = (this._circumcenter - a).magnitude;
            }
            
            // Circumcircle data.
            private readonly Vector2 _circumcenter;
            private readonly float _circumradius;

            // Mesh data.
            public Vector2[] Vertices { get; }
            public Edge[] Edges { get; }
            
            public bool HasEdge(Edge edge)
            {
                for (int i = this.Edges.Length - 1; i >= 0; --i)
                    if (this.Edges[i].EqualsEdge(edge))
                        return true;
                
                return false;
            }

            public bool ShareAnyPoint(Triangle triangle)
            {
                for (int i = triangle.Vertices.Length - 1; i >= 0; --i)
                {
                    Vector2 vertex = triangle.Vertices[i];
                    if (vertex == this.Vertices[0] || vertex == this.Vertices[1] || vertex == this.Vertices[2])
                        return true;
                }

                return false;
            }
            
            public bool IsPointInsideCircumcircle(Vector2 point)
            {
                return (this._circumcenter - point).magnitude <= this._circumradius;
            }
        }

        public readonly struct Edge
        {
            public Edge(Vector2 a, Vector2 b)
            {
                this.A = a;
                this.B = b;
            }
            
            public Vector2 A { get; }
            public Vector2 B { get; }

            public bool EqualsEdge(Edge edge)
            {
                return Vector2.Distance(this.A, edge.A) <= float.Epsilon && Vector2.Distance(this.B, edge.B) <= float.Epsilon
                       || Vector2.Distance(this.A, edge.B) <= float.Epsilon && Vector2.Distance(this.B, edge.A) <= float.Epsilon;
            }
        }

        public struct Triangulation
        {
            public List<Triangle> Triangles;
        }
        
        /// <summary>
        /// Computes a triangle large enough to contain all points that must be triangulated.
        /// </summary>
        /// <param name="points">Points list to triangulate.</param>
        /// <returns>Super-triangle.</returns>
        private Triangle ComputeSuperTriangle(IReadOnlyList<Vector2> points)
        {
            // Compute bounds.
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            for (int i = points.Count - 1; i >= 0; --i)
            {
                Vector2 p = points[i];
                if (minX > p.x) minX = p.x;
                if (minY > p.y) minY = p.y;
                if (maxX < p.x) maxX = p.x;
                if (maxY < p.y) maxY = p.y;
            }
            
            // Compute super triangle vertices.
            float dMax = Mathf.Max(maxX - minX, maxY - minY) * SUPER_TRIANGLE_MARGIN;
            float xCenter = (minX + maxX) * 0.5f;
            float yCenter = (minY + maxY) * 0.5f;

            float x1 = xCenter - 0.866f * dMax;
            float x2 = xCenter + 0.866f * dMax;

            float y1 = yCenter - 0.5f * dMax;
            float y2 = yCenter - 0.5f * dMax;

            Vector2 a = new Vector2(x1, y1);
            Vector2 b = new Vector2(x2, y2);
            Vector2 c = new Vector2(xCenter, yCenter + dMax);

            return new Triangle(a, b, c);
        }
        
        /// <summary>
        /// Computes a Delaunay triangulation using the Bowyer-Watson algorithm.
        /// https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
        /// </summary>
        /// <param name="points">Points list to triangulate.</param>
        /// <returns>Triangulation result.</returns>
        public Triangulation Triangulate(IReadOnlyList<Vector2> points)
        {
            Triangle superTriangle = this.ComputeSuperTriangle(points);
            List<Triangle> triangles = new List<Triangle>() { superTriangle };

            // This algorithm is incremental -> compute each point one by one.
            for (int p = 0; p < points.Count; ++p)
            {
                Vector2 point = points[p];
                List<Triangle> badTriangles = new List<Triangle>();

                // Find all the triangles that are no longer valid due to the insertion.
                for (int i = triangles.Count - 1; i >= 0; --i)
                {
                    Triangle triangle = triangles[i];
                    if (triangle.IsPointInsideCircumcircle(point))
                        badTriangles.Add(triangle);
                }

                // Find the boundary of the polygonal hole.
                List<Edge> polygon = new List<Edge>();
                for (int i = badTriangles.Count - 1; i >= 0; --i)
                {
                    Triangle badTriangle = badTriangles[i];
                    Edge[] edges = badTriangle.Edges;

                    for (int j = edges.Length - 1; j >= 0; --j)
                    {
                        Edge edge = edges[j];
                        bool sharedEdge = false;

                        for (int k = badTriangles.Count - 1; k >= 0; --k)
                        {
                            // Same triangle.
                            if (k == i)
                                continue;
                            
                            sharedEdge = badTriangles[k].HasEdge(edge);
                            if (sharedEdge)
                                break;
                        }

                        if (!sharedEdge)
                            polygon.Add(edge);
                    }
                }

                // Remove bad triangles from the current result list.
                for (int i = badTriangles.Count - 1; i >= 0; --i)
                    triangles.Remove(badTriangles[i]);

                // Re-triangulate the polygonal hole.
                for (int i = polygon.Count - 1; i >= 0; --i)
                    triangles.Add(new Triangle(point, polygon[i].A, polygon[i].B));
            }

            // Remove every triangle that shares a vertex with the super-triangle.
            for (int i = triangles.Count - 1; i >= 0; --i)
            {
                Triangle triangle = triangles[i];
                if (triangle.ShareAnyPoint(superTriangle))
                    triangles.Remove(triangle);
            }

            return new Triangulation
            {
                Triangles = triangles
            };
        }
    }
}
