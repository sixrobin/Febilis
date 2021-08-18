namespace Templar.UI
{
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ItemPickupPanel : MonoBehaviour
    {
        [Header("REFS")]
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private TMPro.TextMeshProUGUI _itemNameText = null;
        [SerializeField] private UnityEngine.UI.Image _itemIcon = null;

        [Header("NOTIFICATION SETTINGS")]
        [SerializeField] private bool _showDialogueItems = true;
        [SerializeField, Min(0f)] private float _maxDuration = 3f;
        [SerializeField, Min(0f)] private float _minItemDuration = 2f;

        private bool _coroutineRunning;
        private System.Collections.Generic.Queue<string> _itemsIdsToShow = new System.Collections.Generic.Queue<string>();

        private void OnInventoryContentChanged(Item.InventoryController.InventoryContentChangedEventArgs args)
        {
            if (args.OnInit
                || args.NewQuantity < args.PrevQuantity
                || args.Item.Datas.SkipPickupNotification
                || Dialogue.DialogueManager.DialogueRunning)
                return;

            AddItemToShow(args.Item.Id);
        }

        private void OnDialogueOver(Datas.Dialogue.DialogueDatas dialogueDatas)
        {
            if (!_showDialogueItems)
                return;

            foreach (Datas.Dialogue.IDialogueSequenceElementDatas dialogueSequenceElement in dialogueDatas.SequenceElementsDatas)
                if (dialogueSequenceElement is Datas.Dialogue.DialogueAddItemDatas addItemDatas)
                    AddItemToShow(addItemDatas.ItemId);
        }

        private void AddItemToShow(string itemId)
        {
            _itemsIdsToShow.Enqueue(itemId);
            if (!_coroutineRunning)
                StartCoroutine(ShowPickedUpItemsCoroutine());
        }

        private System.Collections.IEnumerator ShowPickedUpItemsCoroutine()
        {
            _coroutineRunning = true;
            _canvas.enabled = true;

            while (_itemsIdsToShow.Count > 0)
            {
                string itemId = _itemsIdsToShow.Dequeue();

                _itemNameText.text = itemId; // [TODO] Localization.
                _itemIcon.sprite = Database.ItemDatabase.GetItemSprite(itemId);

                yield return RSLib.Yield.SharedYields.WaitForSeconds(_minItemDuration);
            
                if (_itemsIdsToShow.Count == 0)
                {
                    // Wait longer for the last item, and break if a new item has been added, to display the new one.
                    yield return new RSLib.Yield.WaitForSecondsOrBreakIf(
                        _maxDuration - _minItemDuration,
                        () => _itemsIdsToShow.Count > 0);
                }
            }

            _coroutineRunning = false;
            _canvas.enabled = false;
        }

        private void Start()
        {
            Manager.GameManager.InventoryCtrl.InventoryContentChanged += OnInventoryContentChanged;
            Dialogue.DialogueManager.Instance.DialogueOver += OnDialogueOver;
        }

        private void OnDestroy()
        {
            if (Manager.GameManager.Exists())
                Manager.GameManager.InventoryCtrl.InventoryContentChanged -= OnInventoryContentChanged;
        
            if (Dialogue.DialogueManager.Exists())
                Dialogue.DialogueManager.Instance.DialogueOver -= OnDialogueOver;

        }
    }
}