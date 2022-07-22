namespace Templar.ContextualConditions
{
    using UnityEngine;

    public class ContextualConditionsHandler : MonoBehaviour
    {
        [Header("CONDITIONS GROUP ID")]
        [SerializeField] private string _id = string.Empty;
        
        [Header("CONTEXTUAL EVENTS")]
        [SerializeField] private UnityEngine.Events.UnityEvent _onAllConditionsChecked = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onAnyConditionUnchecked = null;

        private Datas.ContextualConditions.ContextualConditionsDatas _conditionsDatas;
        private IContextualConditionChecker[] _conditionsCheckers;

        private bool _checked;

        private void OnInventoryContentChanged(Item.InventoryController.InventoryContentChangedEventArgs args)
        {
            if (!args.OnInit)
                Check();
        }

        [ContextMenu("Check")]
        public void Check()
        {
            // Should be called with ICheckpointListener call ?

            if (_checked)
                return; // Depending on some param "CanUncheckBack" ?

            if (CheckConditions())
            {
                _onAllConditionsChecked?.Invoke();
                _checked = true;
            }
            else
            {
                _onAnyConditionUnchecked?.Invoke();
                _checked = false;
            }
        }

        public bool CheckConditions()
        {
            for (int i = _conditionsCheckers.Length - 1; i >= 0; --i)
                if (!_conditionsCheckers[i].Check())
                    return false;

            return true;
        }

        private void CreateConditionsCheckers()
        {
            _conditionsCheckers = new IContextualConditionChecker[_conditionsDatas.ConditionsDatas.Length];
            for (int i = _conditionsDatas.ConditionsDatas.Length - 1; i >= 0; --i)
            {
                if (_conditionsDatas.ConditionsDatas[i] is Datas.ContextualConditions.HasItemContextualConditionDatas hasItemConditionDatas)
                {
                    _conditionsCheckers[i] = new HasItemContextualConditionChecker(hasItemConditionDatas);
                }
                else if (_conditionsDatas.ConditionsDatas[i] is Datas.ContextualConditions.DoesntHaveItemContextualConditionDatas doesntHaveItemConditionDatas)
                {
                    _conditionsCheckers[i] = new DoesntHaveItemContextualConditionChecker(doesntHaveItemConditionDatas);
                }
                else
                {
                    CProLogger.LogError(this, $"Unhandled Contextual Condition checker for condition type {_conditionsDatas.ConditionsDatas[i].GetType().Name}.", gameObject);
                    break;
                }
            }
        }

        private void Start()
        {
            Manager.GameManager.InventoryCtrl.InventoryContentChanged += OnInventoryContentChanged;
               
            UnityEngine.Assertions.Assert.IsTrue(
                Database.ContextualConditionsDatabase.ContextualConditionsDatas.ContainsKey(_id),
                $"Unknown ContextualConditions group with Id {_id}.");

            _conditionsDatas = Database.ContextualConditionsDatabase.ContextualConditionsDatas[_id];
            CreateConditionsCheckers();

            _onAnyConditionUnchecked?.Invoke(); // Consider all conditions are false on start, then check them.
            Check(); // Reset values so that we don't have to do it in the editor each time.
        }

        private void OnDestroy()
        {
            if (Manager.GameManager.Exists())
                Manager.GameManager.InventoryCtrl.InventoryContentChanged -= OnInventoryContentChanged;
        }
    }
}