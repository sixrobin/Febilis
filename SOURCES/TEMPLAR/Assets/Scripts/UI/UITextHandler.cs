namespace Templar.UI
{
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class UITextHandler : MonoBehaviour, RSLib.Framework.GUI.IUIVisibleEventListener
    {
        [SerializeField] private ColorByZone[] _colorsByZone = null;

        private TMPro.TextMeshProUGUI _text;
        private Color? _defaultColor;

        public GameObject GameObject => gameObject;
        
        void RSLib.Framework.GUI.IUIVisibleEventListener.OnUIVisibleChanged(bool visible)
        {
            if (visible)
                RefreshColor();
        }

        [ContextMenu("Refresh Color")]
        private void RefreshColor()
        {
            if (_text == null)
            {
                _text = GetComponent<TMPro.TextMeshProUGUI>();
                if (_text == null)
                {
                    CProLogger.LogWarning(this, $"Missing text reference on {transform.name}!", gameObject);
                    return;
                }
            }
         
            if (_defaultColor == null)
                _defaultColor = _text.color;
            
            Flags.ZoneIdentifier currentZone = Manager.BoardsManager.CurrentBoard != null ? Manager.BoardsManager.CurrentBoard.BoardIdentifier.ContainingZoneIdentifier : null;
            _text.color = currentZone != null
                          ? _colorsByZone.FirstOrDefault(o => o.Zone == currentZone).DataColor
                          : _defaultColor.Value;
        }

        private void OnEnable()
        {
            RefreshColor();
            RSLib.Framework.GUI.UIVisibleEventHandler.Register(this);
        }

        private void OnDisable()
        {
            RSLib.Framework.GUI.UIVisibleEventHandler.Unregister(this);
        }
    }
}
