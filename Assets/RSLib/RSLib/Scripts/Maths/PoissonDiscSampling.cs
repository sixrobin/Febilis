namespace RSLib.Maths
{
    using UnityEngine;

    public class PoissonDiscSampling
    {
        private const float DEFAULT_RADIUS = 0.1f;
        
        public PoissonDiscSampling(float radius, Vector2 sampleBox, int rejectionSamplesCount)
        {
            if (radius <= 0f)
            {
                Debug.LogError($"{nameof(PoissonDiscSampling)} radius must be positive! Setting it to {DEFAULT_RADIUS} (tried to set it to {radius}).");
                _radius = DEFAULT_RADIUS;
            }
            
            _radius = radius;
            _cellSize = _radius / Mathf.Sqrt(2f);
            _sampleBox = sampleBox;
            _rejectionSamplesCount = rejectionSamplesCount;
        }

        private float _radius;
        private float _cellSize;
        private Vector2 _sampleBox;
        private int _rejectionSamplesCount;

        public System.Collections.Generic.List<Vector2> Sample()
        {
            int[,] grid = new int[Mathf.CeilToInt(_sampleBox.x / _cellSize), Mathf.CeilToInt(_sampleBox.y / _cellSize)];

            System.Collections.Generic.List<Vector2> points = new System.Collections.Generic.List<Vector2>();
            System.Collections.Generic.List<Vector2> spawnPoints = new System.Collections.Generic.List<Vector2>();
            
            spawnPoints.Add(_sampleBox * 0.5f);

            while (spawnPoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, spawnPoints.Count);
                Vector2 spawnCenter = spawnPoints[spawnIndex];

                bool validCandidate = false;

                for (int i = 0; i < _rejectionSamplesCount; ++i)
                {
                    float angle = Random.value * Mathf.PI * 2f;
                    Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    Vector2 candidate = spawnCenter + (direction * Random.Range(_radius, _radius * 2f));

                    if (IsValidCandidate(candidate, points, grid))
                    {
                        points.Add(candidate);
                        spawnPoints.Add(candidate);
                        grid[(int)(candidate.x / _cellSize), (int)(candidate.y / _cellSize)] = points.Count;
                        validCandidate = true;
                        
                        break;
                    }
                }

                if (!validCandidate)
                {
                    spawnPoints.RemoveAt(spawnIndex);
                }
            }

            return points;
        }

        private bool IsValidCandidate(Vector2 candidate, System.Collections.Generic.List<Vector2> points, int[,] grid)
        {
            if (candidate.x < 0f || candidate.x >= this._sampleBox.x || candidate.y < 0 || candidate.y >= this._sampleBox.y)
                return false;
            
            int cellX = (int)(candidate.x / this._cellSize);
            int cellY = (int)(candidate.y / this._cellSize);
                
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; ++x)
            {
                for (int y = searchStartY; y <= searchEndY; ++y)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1 && (candidate - points[pointIndex]).sqrMagnitude < this._radius * this._radius)
                        return false;
                }
            }

            return true;
        }
    }
}
