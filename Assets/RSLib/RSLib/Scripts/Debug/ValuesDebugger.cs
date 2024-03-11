namespace RSLib.Debug
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif
    
    public static class AnchorExtensions
    {
        public static bool IsRight(this ValuesDebugger.Anchor anchor)
        {
            return anchor == ValuesDebugger.Anchor.LOWER_RIGHT || anchor == ValuesDebugger.Anchor.UPPER_RIGHT;
        }

        public static bool IsUp(this ValuesDebugger.Anchor anchor)
        {
            return anchor == ValuesDebugger.Anchor.UPPER_LEFT || anchor == ValuesDebugger.Anchor.UPPER_RIGHT;
        }
    }

    [DisallowMultipleComponent]
    public sealed class ValuesDebugger : Framework.Singleton<ValuesDebugger>
    {
        private const float LINE_HEIGHT = 20f;
        private const string DEFAULT_FORMAT = "{0}:{1}";
        
        #if !ODIN_INSPECTOR
        [Header("DATA")]
        #endif
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Data")]
        #endif
        [SerializeField] private KeyCode _toggleKey = KeyCode.F1;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Data")]
        #endif
        [SerializeField] private bool _buildEnabled = false;

        #if !ODIN_INSPECTOR
        [Header("STYLE")]
        #endif
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField] private string _format = DEFAULT_FORMAT;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField, Min(0f)] private float _margin = 0f;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField, Min(0f)] private float _linesHeight = 15f;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField] private Color _textsColor = Color.yellow;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField] private bool _boldFont = true;

        private Dictionary<Anchor, Dictionary<string, ValueGetter>> _values = new Dictionary<Anchor, Dictionary<string, ValueGetter>>();
        private Dictionary<Anchor, GUIStyle> _styles = new Dictionary<Anchor, GUIStyle>();

        private bool _enabled;

        // FPS display.
        private float _lastInterval;
        private float _frames;
        
        public delegate object ValueGetter();

        public enum Anchor
        {
            UPPER_RIGHT,
            UPPER_LEFT,
            LOWER_RIGHT,
            LOWER_LEFT
        }

        public static void DebugValue(string key, ValueGetter valueGetter, Anchor anchor = Anchor.UPPER_RIGHT)
        {
            if (!Exists())
                return;

            if (!Instance._values.ContainsKey(anchor))
                Instance._values.Add(anchor, new Dictionary<string, ValueGetter>());

            if (Instance._values[anchor].ContainsKey(key))
                Instance._values[anchor][key] = valueGetter;
            else
                Instance._values[anchor].Add(key, valueGetter);
        }

        public static void DebugValue(ValueGetter valueGetter, Anchor anchor = Anchor.UPPER_RIGHT)
        {
            DebugValue(string.Empty, valueGetter, anchor);
        }
        
        public void Enable(bool state)
        {
            _buildEnabled = state;
            if (!state && !Application.isEditor)
                _enabled = false;
        }
        
        private static void ClearValues()
        {
            foreach (KeyValuePair<Anchor, Dictionary<string, ValueGetter>> values in Instance._values)
            {
                string[] keys = values.Value.Keys.ToArray();
                for (int i = keys.Length - 1; i >= 0; --i)
                {
                    object target = values.Value[keys[i]].Target;
                    if (target == null || (target is Object && target.Equals(null)))
                        values.Value.Remove(keys[i]);
                }
            }
        }

        private void InitGUIStyles()
        {
            GUIStyle upperLeftStyle = new GUIStyle()
            {
                fontStyle = _boldFont ? FontStyle.Bold : FontStyle.Normal,
                alignment = TextAnchor.UpperLeft
            };

            GUIStyle upperRightStyle = new GUIStyle()
            {
                fontStyle = _boldFont ? FontStyle.Bold : FontStyle.Normal,
                alignment = TextAnchor.UpperRight
            };

            GUIStyle lowerLeftStyle = new GUIStyle()
            {
                fontStyle = _boldFont ? FontStyle.Bold : FontStyle.Normal,
                alignment = TextAnchor.LowerLeft
            };

            GUIStyle lowerRightStyle = new GUIStyle()
            {
                fontStyle = _boldFont ? FontStyle.Bold : FontStyle.Normal,
                alignment = TextAnchor.LowerRight
            };

            upperLeftStyle.normal.textColor = _textsColor;
            upperRightStyle.normal.textColor = _textsColor;
            lowerLeftStyle.normal.textColor = _textsColor;
            lowerRightStyle.normal.textColor = _textsColor;

            _styles = new Dictionary<Anchor, GUIStyle>()
            {
                { Anchor.UPPER_LEFT, upperLeftStyle },
                { Anchor.UPPER_RIGHT, upperRightStyle },
                { Anchor.LOWER_LEFT, lowerLeftStyle },
                { Anchor.LOWER_RIGHT, lowerRightStyle }
            };
        }

        private void UpdateFPSCounter()
        {
            _frames++;
            float realtimeSinceStartup = Time.realtimeSinceStartup;

            const float updateInterval = 0.5f;
            if (_lastInterval + updateInterval >= realtimeSinceStartup)
                return;
            
            float fps = _frames / (realtimeSinceStartup - _lastInterval);
            float ms = 1000f / Mathf.Max(fps, 0.00001f);
            
            DebugValue(() => $"{fps:f2}FPS ({ms:f1}ms)", Anchor.LOWER_RIGHT);

            _frames = 0;
            _lastInterval = realtimeSinceStartup;
        }
        
        protected override void Awake()
        {
            base.Awake();
            InitGUIStyles();
            
            _lastInterval = Time.realtimeSinceStartup;
            _frames = 0;
        }

        private void Update()
        {
            if ((_enabled || Application.isEditor || _buildEnabled)
                && Input.GetKeyDown(_toggleKey))
            {
                _enabled = !_enabled;
            }
            
            UpdateFPSCounter();
        }

        private void OnGUI()
        {
            if (!_enabled)
                return;

            ClearValues();

            foreach (KeyValuePair<Anchor, Dictionary<string, ValueGetter>> values in _values)
            {
                Anchor anchor = values.Key;
                int i = 0;
                
                foreach (KeyValuePair<string, ValueGetter> entry in values.Value)
                {
                    Vector2 rectPos = new Vector2()
                    {
                        x = _margin,
                        y = anchor.IsUp() ? (_linesHeight * i + _margin) : (Screen.height - _linesHeight * i - _margin - LINE_HEIGHT)
                    };

                    Vector2 rectSize = new Vector2()
                    {
                        x = Screen.width - (_margin * 2),
                        y = LINE_HEIGHT
                    };

                    Rect rect = new Rect(rectPos, rectSize);

                    try
                    {
                        if (string.IsNullOrEmpty(entry.Key))
                            GUI.TextField(rect, entry.Value().ToString(), _styles[values.Key]);
                        else
                            GUI.TextField(rect, string.Format(_format, entry.Key, entry.Value()), _styles[values.Key]);
                    }
                    catch (System.Exception e)
                    {
                        LogError($"Exception caught while debugging value, using default format {DEFAULT_FORMAT}. Exception message : {e.Message}");
                        GUI.TextField(rect, string.Format(DEFAULT_FORMAT, entry.Key, entry.Value()), _styles[values.Key]);
                    }
                    
                    i++;
                }
            }
        }

        private void OnValidate()
        {
            InitGUIStyles();
        }
    }
}