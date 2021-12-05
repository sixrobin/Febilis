namespace Templar.MainMenu
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;

    public class MainMenuPanel : UI.UIPanel
    {
        [Header("BUTTONS")]
        [SerializeField] private GameObject _btnsContainer = null;
        [SerializeField] private UnityEngine.UI.Selectable _continueBtn = null;
        [SerializeField] private UnityEngine.UI.Selectable _newGameBtn = null;
        [SerializeField] private UnityEngine.UI.Selectable _settingsBtn = null;
        [SerializeField] private UnityEngine.UI.Selectable _quitBtn = null;

        private UnityEngine.UI.Selectable[] _allButtons;

        public override GameObject FirstSelected => Manager.SaveManager.GameSaveExist ? _continueBtn.gameObject : _newGameBtn.gameObject;

        public override void OnBackButtonPressed()
        {
        }

        private void InitButtonsState()
        {
            bool gameSaveExist = Manager.SaveManager.GameSaveExist;
            _continueBtn.gameObject.SetActive(gameSaveExist);
        }

        private void InitButtonsEvents()
        {
            throw new System.NotImplementedException();
            // [TODO] Btns should be actual Buttons/EnhancedButtons to get the OnClick.AddListener() method.
        }

        private void InitNavigation()
        {
            System.Collections.Generic.List<UnityEngine.UI.Selectable> enabledBtns = _allButtons.Where(o => o.gameObject.activeSelf).ToList();
            enabledBtns.Sort((UnityEngine.UI.Selectable a, UnityEngine.UI.Selectable b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

            for (int i = enabledBtns.Count - 1; i >= 0; --i)
            {
                enabledBtns[i].SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                enabledBtns[i].SetSelectOnUp(enabledBtns[RSLib.Helpers.Mod(i - 1, enabledBtns.Count)]);
                enabledBtns[i].SetSelectOnDown(enabledBtns[RSLib.Helpers.Mod(i + 1, enabledBtns.Count)]);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _allButtons = new UnityEngine.UI.Selectable[]
            {
                _continueBtn,
                _newGameBtn,
                _settingsBtn,
                _quitBtn
            };

            InitButtonsState();
            InitButtonsEvents();
            InitNavigation();
        }

        private void Start()
        {
            UI.Navigation.UINavigationManager.OpenAndSelect(this);
        }
    }
}