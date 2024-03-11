namespace RSLib.Editor
{
    using Extensions;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public static class TilemapEditorToolsMenu
    {
        [MenuItem("RSLib/Tilemap Tools")]
        public static void LaunchTilemapUtilities()
        {
            TilemapEditorTools.LaunchTilemapUtilities();
        }
    }

    public sealed class TilemapEditorTools : EditorWindow
    {
        private const float BUTTON_HEIGHT = 30f;
        private const float CATEGORIES_SPACING = 20f;
        
        private static bool s_firstOpenFrame = true;

        // Override tiles.
        private Tilemap _tilemapToOverrideTiles;
        private Tilemap _overriddenTilemap;
        private TileBase _tile;

        // Clear tilemap.
        private Tilemap _tilemapToClear;

        // Clear alone tiles.
        private Tilemap _tilemapToClearAloneTiles;
        private bool _clearAloneIgnoreDiagonals;

        // Carve tilemap.
        private Tilemap _tilemapToCarve;
        private Tilemap _carveShape;
        
        // Clear tilemap collision.
        private Tilemap _tilemapToClearCollision;
        
        public static void LaunchTilemapUtilities()
        {
            s_firstOpenFrame = true;

            EditorWindow window = GetWindow<TilemapEditorTools>("Tilemap Utilities");
            window.Show();
        }

        private static void ClearTiles(Tilemap tilemap)
        {
            tilemap.ClearAllTiles();
            tilemap.ClearAllEditorPreviewTiles();
        }

        private void OverrideTilesToNewTilemap()
        {
            Tilemap copy = _tilemapToOverrideTiles.OverrideTilesToNewTilemap(_tile);
            Selection.activeGameObject = copy.gameObject;
        }

        private void OnGUI()
        {
            if (s_firstOpenFrame)
            {
                Tilemap selectedTilemap = Selection.activeGameObject?.GetComponent<Tilemap>();

                _tilemapToOverrideTiles = selectedTilemap;
                _tilemapToClear = selectedTilemap;
                _tilemapToClearAloneTiles = selectedTilemap;
                s_firstOpenFrame = false;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10f);


            // Override tiles.
            {
                EditorGUILayout.LabelField("OVERRIDE TILEMAP TILES", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tilemap to override tiles of");
                _tilemapToOverrideTiles = EditorGUILayout.ObjectField(_tilemapToOverrideTiles, typeof(Tilemap), true, null) as Tilemap;
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override tile");
                _tile = EditorGUILayout.ObjectField(_tile, typeof(TileBase), true, null) as TileBase;
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);

                if (GUILayout.Button("Override Tiles to new Tilemap", GUILayout.Height(BUTTON_HEIGHT), GUILayout.ExpandWidth(true)))
                {
                    if (_tilemapToOverrideTiles == null)
                    {
                        EditorUtility.DisplayDialog("Tilemap Utilities Warning", "You must provide a source Tilemap for tiles positions!", "OK");
                        return;
                    }

                    // Use name and not type directly because 2d extras can be missing in the project, causing errors with those types.
                    string tileTypeName = _tile != null ? _tile.GetType().Name : string.Empty;
                    if (tileTypeName == "RuleTile" || tileTypeName == "RuleOverrideTile")
                    {
                        OverrideTilesToNewTilemap();
                    }
                    else if (_tile != null
                        && EditorUtility.DisplayDialog(
                        "Tilemap Utilities Warning",
                        "Referenced tile is not a RuleTile. Are you sure you want to fill a tilemap with a non ruled tile?",
                        "Yes",
                        "No"))
                    {
                        OverrideTilesToNewTilemap();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Tilemap Utilities Warning", "You must provide a tile to create new Tilemap!", "OK");
                        return;
                    }
                }

                GUILayout.Space(2f);
                EditorGUILayout.EndVertical();
            }
            
            GUILayout.Space(CATEGORIES_SPACING);

            // Clear tilemap.
            {
                EditorGUILayout.LabelField("CLEAR TILEMAP (NO UNDO)", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tilemap to clear");
                _tilemapToClear = EditorGUILayout.ObjectField(_tilemapToClear, typeof(Tilemap), true, null) as Tilemap;
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(5f);

                if (GUILayout.Button("Clear Tiles", GUILayout.Height(BUTTON_HEIGHT), GUILayout.ExpandWidth(true)))
                {
                    if (_tilemapToClear == null)
                    {
                        EditorUtility.DisplayDialog("Tilemap Utilities Warning", "You must provide a Tilemap to clear its tiles!", "OK");
                        return;
                    }

                    ClearTiles(_tilemapToClear);
                }

                GUILayout.Space(2f);
                EditorGUILayout.EndVertical();
            }
            
            GUILayout.Space(CATEGORIES_SPACING);

            // Clear alone tiles.
            {
                EditorGUILayout.LabelField("CLEAR ALONE TILES (NO UNDO)", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tilemap to clear isolated tiles of");
                _tilemapToClearAloneTiles = EditorGUILayout.ObjectField(_tilemapToClearAloneTiles, typeof(Tilemap), true, null) as Tilemap;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Ignore diagonals");
                _clearAloneIgnoreDiagonals = EditorGUILayout.Toggle(_clearAloneIgnoreDiagonals);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);

                if (GUILayout.Button("Clear Alone Tiles", GUILayout.Height(BUTTON_HEIGHT), GUILayout.ExpandWidth(true)))
                {
                    if (_tilemapToClearAloneTiles == null)
                    {
                        EditorUtility.DisplayDialog("Tilemap Utilities Warning", "You must provide a Tilemap to clear its alone tiles!", "OK");
                        return;
                    }

                    _tilemapToClearAloneTiles.ClearAloneTiles(_clearAloneIgnoreDiagonals);
                }

                GUILayout.Space(2f);
                EditorGUILayout.EndVertical();
            }
            
            GUILayout.Space(CATEGORIES_SPACING);

            // Carve tilemap.
            {
                EditorGUILayout.LabelField("CARVE TILEMAP (NO UNDO)", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tilemap to carve");
                _tilemapToCarve = EditorGUILayout.ObjectField(_tilemapToCarve, typeof(Tilemap), true, null) as Tilemap;
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Carve shape");
                _carveShape = EditorGUILayout.ObjectField(_carveShape, typeof(Tilemap), true, null) as Tilemap;
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(5f);

                if (GUILayout.Button("Carve tilemap", GUILayout.Height(BUTTON_HEIGHT), GUILayout.ExpandWidth(true)))
                {
                    if (_tilemapToCarve == null)
                    {
                        EditorUtility.DisplayDialog("Tilemap Utilities Warning", "You must provide a Tilemap to carve!", "OK");
                        return;
                    }

                    if (_carveShape == null)
                    {
                        EditorUtility.DisplayDialog("Tilemap Utilities Warning", "You must provide a Tilemap as a shape reference to carve!", "OK");
                        return;
                    }
                    
                    _tilemapToCarve.CarveTilemap(_carveShape);
                }
                
                GUILayout.Space(2f);
                EditorGUILayout.EndVertical();
            }
            
            GUILayout.Space(CATEGORIES_SPACING);

            // Clear tilemap collision.
            {
                EditorGUILayout.LabelField("CLEAR TILEMAP COLLISION (NO UNDO)", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tilemap to clear collision of");
                _tilemapToClearCollision = EditorGUILayout.ObjectField(_tilemapToClearCollision, typeof(Tilemap), true, null) as Tilemap;
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(5f);

                if (GUILayout.Button("Clear tilemap collision", GUILayout.Height(BUTTON_HEIGHT), GUILayout.ExpandWidth(true)))
                {
                    if (_tilemapToClearCollision == null)
                    {
                        EditorUtility.DisplayDialog("Tilemap Utilities Warning", "You must provide a Tilemap to clear its collision!", "OK");
                        return;
                    }
                    
                    _tilemapToClearCollision.TryRemoveCollision(true);
                }
                
                GUILayout.Space(2f);
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndVertical();
            GUILayout.Space(10f);
            EditorGUILayout.EndHorizontal();

            Repaint();
        }
    }
}