﻿namespace Templar.UI.Settings
{
    using UnityEngine;

    public abstract class SettingView : UnityEngine.UI.Selectable, UnityEngine.EventSystems.ISelectHandler
    {
        [Header("SETTING VIEW BASE")]
        [SerializeField] private TMPro.TextMeshProUGUI _title = null;
        [SerializeField] private string _titleLocalizationKey = string.Empty;
        [SerializeField] private RSLib.Framework.GUI.PointerEventsHandler _pointerEventsHandler = null;

        public abstract Templar.Settings.Setting Setting { get; }

        public RSLib.Framework.GUI.PointerEventsHandler PointerEventsHandler => _pointerEventsHandler;

        public abstract UnityEngine.UI.Selectable Selectable { get; }

        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            PointerEventsHandler.OnPointerEnter(null);

            Navigation.UINavigationManager.Select(Selectable.gameObject);
            
            RSLib.Audio.UI.UIAudioManager.PlayHoverClip();
        }

        public virtual void Init()
        {
            InitSelectable();
            Localize();
        }

        public abstract void InitSelectable();
        
        public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            PointerEventsHandler.OnPointerEnter(eventData);
            
            RSLib.Audio.UI.UIAudioManager.PlayHoverClip();
        }

        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            PointerEventsHandler.OnPointerExit(eventData);
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            PointerEventsHandler.OnPointerDown(eventData);
        }

        public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            PointerEventsHandler.OnPointerUp(eventData);
        }

        public virtual void Localize()
        {
            _title.text = RSLib.Localization.Localizer.Get(_titleLocalizationKey);
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            transition = Transition.None;
            _pointerEventsHandler = GetComponentInChildren<RSLib.Framework.GUI.PointerEventsHandler>();
        }
#endif
    }
}