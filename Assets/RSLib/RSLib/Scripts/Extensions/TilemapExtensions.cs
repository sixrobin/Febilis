namespace RSLib.Extensions
{
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public static class TilemapExtensions
    {
        /// <summary>
        /// Carves a tilemap based on another tilemap shape.
        /// Can be used to define secret corridors shape on some invisible tilemap to carve a visible one.
        /// </summary>
        /// <param name="tilemap">Tilemap to carve.</param>
        /// <param name="carveShape">Carve shape.</param>
        public static void CarveTilemap(this Tilemap tilemap, Tilemap carveShape)
        {
            foreach (Vector3Int position in carveShape.cellBounds.allPositionsWithin)
                if (carveShape.HasTile(position))
                    tilemap.SetTile(position, null);
        }
        
        /// <summary>
        /// Clears tiles that are isolated on a tilemap.
        /// </summary>
        /// <param name="tilemap">Tilemap to clear.</param>
        /// <param name="ignoreDiagonals">Do not include diagonals during isolated check.</param>
        public static void ClearAloneTiles(this Tilemap tilemap, bool ignoreDiagonals)
        {
            System.Collections.Generic.List<Vector3Int> aloneTiles = new System.Collections.Generic.List<Vector3Int>();

            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(pos))
                    continue;

                for (int x = -1; x <= 1; ++x)
                {
                    for (int y = -1; y <= 1; ++y)
                    {
                        if (x == 0 && y == 0
                            || Mathf.Abs(x) + Mathf.Abs(y) == 2 && ignoreDiagonals)
                            continue;

                        if (tilemap.HasTile(new Vector3Int(pos.x + x, pos.y + y, 0)))
                            goto NextTile;
                    }
                }

                aloneTiles.Add(pos);

                NextTile:
                continue;
            }

            foreach (Vector3Int aloneTilePos in aloneTiles)
                tilemap.SetTile(aloneTilePos, null);

            Debug.Log($"Cleared {aloneTiles.Count} alone tiles on {tilemap.transform.name} tilemap.");
        }

        /// <summary>
        /// Replaces every tile on a tilemap with a specified tile.
        /// Should probably be used with an auto-tiling tile.
        /// </summary>
        /// <param name="tilemap">Tilemap to replace tiles of.</param>
        /// <param name="tileBase">Replacing tile.</param>
        public static void OverrideTiles(this Tilemap tilemap, TileBase tileBase)
        {
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
                if (tilemap.HasTile(pos))
                    tilemap.SetTile(pos, tileBase);
        }
        
        /// <summary>
        /// Creates a tilemap copy and replaces its tiles with a specified tile.
        /// Should probably be used with an auto-tiling tile.
        /// </summary>
        /// <param name="tilemap">Tilemap to copy and replace the tiles of.</param>
        /// <param name="tileBase">Replacing tile.</param>
        public static Tilemap OverrideTilesToNewTilemap(this Tilemap tilemap, TileBase tileBase)
        {
            Tilemap copy = Object.Instantiate(tilemap, tilemap.transform.parent);
            copy.name = tilemap.name + "_Copy";
            copy.ClearAllTiles();

            int tilesCounter = 0;

            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(pos))
                    continue;
                
                copy.SetTile(pos, tileBase);
                tilesCounter++;
            }

            if (tilesCounter == 0)
            {
                Debug.LogWarning($"No tile has been found on source Tilemap {tilemap} to create a new Tilemap.", tilemap.gameObject);
                Object.DestroyImmediate(copy.gameObject);
                return null;
            }

            Debug.Log($"Created new Tilemap {copy.name} with {tilesCounter} overridden tiles.", copy.gameObject);
            return copy;
        }

        /// <summary>
        /// Deletes collision related component on a Tilemap if some are found.
        /// </summary>
        /// <param name="tilemap">Tilemap to remove collision of.</param>
        public static void TryRemoveCollision(this Tilemap tilemap, bool destroyImmediate = false)
        {
            if (tilemap.TryGetComponent(out TilemapCollider2D tilemapCollider2D))
            {
                if (destroyImmediate)
                    Object.DestroyImmediate(tilemapCollider2D);
                else                    
                    Object.Destroy(tilemapCollider2D);
            }

            if (tilemap.TryGetComponent(out Rigidbody2D rigidbody2D) && tilemap.TryGetComponent(out CompositeCollider2D compositeCollider2D))
            {
                if (destroyImmediate)
                {
                    Object.DestroyImmediate(compositeCollider2D);
                    Object.DestroyImmediate(rigidbody2D);
                }
                else
                {
                    Object.Destroy(compositeCollider2D);
                    Object.Destroy(rigidbody2D);
                }
            }
        }
    }
}
