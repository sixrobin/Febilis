namespace Templar.UI
{
    using UnityEngine;

    public class HUDElement : MonoBehaviour
    {
        [Header("HUD BASE")]
        [SerializeField] private Canvas _canvas = null;

        protected virtual void OnFadeBegan(bool fadeIn)
        {
            Display(false);
        }

        protected virtual void OnFadeOver(bool fadeIn)
        {
            if (fadeIn)
                return;

            Display(true);
        }

        protected virtual void OnOptionsOpened()
        {
            Display(false);
        }

        protected virtual void OnOptionsClosed()
        {
            if (!Manager.GameManager.PlayerCtrl.IsDead
                && !Manager.BoardsTransitionManager.IsInBoardTransition
                && !Manager.RampFadeManager.IsFading)
                Display(true);
        }

        protected virtual void OnSleepAnimationBegan()
        {
            Display(false);
        }

        protected virtual void OnSleepAnimationOver()
        {
            Display(true);
        }

        protected virtual void OnInventoryDisplayChanged(bool displayed)
        {
            Display(!displayed);
        }

        protected virtual void OnInventoryContentChanged(Item.InventoryController.InventoryContentChangedEventArgs args)
        {
        }

        protected virtual void OnDialogueStarted(Datas.Dialogue.DialogueDatas dialogueDatas)
        {
            Display(false);
        }

        protected virtual void OnDialogueOver(Datas.Dialogue.DialogueDatas dialogueDatas)
        {
            Display(true);
        }
        
        protected virtual void OnBossFightWonCutsceneStarted()
        {
            Display(false);
        }
        
        protected virtual void OnBossFightWonCutsceneOver()
        {
            Display(true);
        }

        protected virtual void Display(bool state)
        {
            if (state && !CanBeDisplayed())
                return;

            _canvas.enabled = state;
        }

        protected virtual bool CanBeDisplayed()
        {
            return !Manager.GameManager.PlayerCtrl.IsDead;
        }

        protected virtual void Awake()
        {
            Manager.RampFadeManager.Instance.FadeBegan += OnFadeBegan;
            Manager.RampFadeManager.Instance.FadeOver += OnFadeOver;

            Manager.OptionsManager.Instance.OptionsOpened += OnOptionsOpened;
            Manager.OptionsManager.Instance.OptionsClosed += OnOptionsClosed;

            Manager.GameManager.PlayerCtrl.PlayerView.SleepAnimationBegan += OnSleepAnimationBegan;
            Manager.GameManager.PlayerCtrl.PlayerView.SleepAnimationOver += OnSleepAnimationOver;

            if (Manager.GameManager.InventoryView != null)
                Manager.GameManager.InventoryView.DisplayChanged += OnInventoryDisplayChanged;

            if (Manager.GameManager.InventoryCtrl != null)
                Manager.GameManager.InventoryCtrl.InventoryContentChanged += OnInventoryContentChanged;

            UI.Dialogue.DialogueManager.Instance.DialogueStarted += OnDialogueStarted;
            UI.Dialogue.DialogueManager.Instance.DialogueOver += OnDialogueOver;

            Templar.Boss.BossFightWonCutscene.CutsceneStarted += OnBossFightWonCutsceneStarted;
            Templar.Boss.BossFightWonCutscene.CutsceneOver += OnBossFightWonCutsceneOver;
        }

        protected virtual void OnDestroy()
        {
            if (Manager.RampFadeManager.Exists())
            {
                Manager.RampFadeManager.Instance.FadeBegan -= OnFadeBegan;
                Manager.RampFadeManager.Instance.FadeOver -= OnFadeOver;
            }

            if (Manager.OptionsManager.Exists())
            {
                Manager.OptionsManager.Instance.OptionsOpened -= OnOptionsOpened;
                Manager.OptionsManager.Instance.OptionsClosed -= OnOptionsClosed;
            }

            if (Manager.GameManager.Exists())
            {
                if (Manager.GameManager.InventoryView != null)
                    Manager.GameManager.InventoryView.DisplayChanged -= OnInventoryDisplayChanged;

                if (Manager.GameManager.InventoryCtrl != null)
                    Manager.GameManager.InventoryCtrl.InventoryContentChanged -= OnInventoryContentChanged;

                if (Manager.GameManager.PlayerCtrl != null)
                {
                    Manager.GameManager.PlayerCtrl.PlayerView.SleepAnimationBegan -= OnSleepAnimationBegan;
                    Manager.GameManager.PlayerCtrl.PlayerView.SleepAnimationOver -= OnSleepAnimationOver;
                }
            }

            if (UI.Dialogue.DialogueManager.Exists())
            {
                UI.Dialogue.DialogueManager.Instance.DialogueStarted -= OnDialogueStarted;
                UI.Dialogue.DialogueManager.Instance.DialogueOver -= OnDialogueOver;
            }

            if (Templar.Boss.BossFightWonCutscene.Exists())
            {
                Templar.Boss.BossFightWonCutscene.CutsceneStarted -= OnBossFightWonCutsceneStarted;
                Templar.Boss.BossFightWonCutscene.CutsceneOver -= OnBossFightWonCutsceneOver;
            }
        }
    }
}