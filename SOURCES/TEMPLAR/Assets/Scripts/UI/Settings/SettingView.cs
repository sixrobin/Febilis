namespace Templar.UI.Settings
{
    using UnityEngine;

    public abstract class SettingView : UnityEngine.UI.Selectable, UnityEngine.EventSystems.ISelectHandler
    {
        [Header("SETTING VIEW BASE")]
        [SerializeField] private bool _visible = true;

        public bool Visible => _visible;
        public abstract UnityEngine.UI.Selectable Selectable { get; }

        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            Navigation.UINavigationManager.Select(Selectable.gameObject);
        }

        public abstract void Init();

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            transition = Transition.None;
        }
#endif
    }
}