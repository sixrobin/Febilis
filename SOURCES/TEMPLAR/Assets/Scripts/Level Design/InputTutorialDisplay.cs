namespace Templar
{
    using UnityEngine;
    
    public class InputTutorialDisplay : MonoBehaviour
    {
        [SerializeField] private ValidationType _validationType = ValidationType.TRIGGER_ENTER;
        [SerializeField] private RSLib.Framework.OptionalBoxCollider2D _validatingTrigger = new RSLib.Framework.OptionalBoxCollider2D(null, false);

        private bool _validated;
        
        private enum ValidationType
        {
            TRIGGER_ENTER,
            MOVEMENT,
            ATTACK,
            ROLL,
            EFFECTOR,
            HEAL,
            INTERACTION,
            INVENTORY
        }

        private void RaiseValidationEvent(ValidationType validationType)
        {
            if (validationType != _validationType || _validated)
                return;
            
            _validated = true;
            gameObject.SetActive(false);
        }
        
        private System.Collections.IEnumerator InitEvents()
        {
            Unit.Player.PlayerController playerCtrl = Manager.GameManager.PlayerCtrl;
            yield return new WaitUntil(() => playerCtrl.Initialized);
            
            // TODO: Only register events fitting specified validation type of each instance.
            
            playerCtrl.AttackCtrl.AttackHitTriggered += () => RaiseValidationEvent(ValidationType.ATTACK);
            playerCtrl.RollCtrl.Rolled += () => RaiseValidationEvent(ValidationType.ROLL);
            playerCtrl.CollisionsCtrl.EffectorDown += (effector) => RaiseValidationEvent(ValidationType.EFFECTOR);
            playerCtrl.Interacter.Interacted += (interactable) => RaiseValidationEvent(ValidationType.INTERACTION);

            playerCtrl.HealthCtrl.UnitHealthChanged += (args) =>
            {
                if (!args.IsLoss)
                    RaiseValidationEvent(ValidationType.HEAL);
            };
            
            Manager.GameManager.InventoryView.DisplayChanged += (displayed) =>
            {
                if (displayed)
                    RaiseValidationEvent(ValidationType.INVENTORY);
            };

            if (_validationType == ValidationType.TRIGGER_ENTER)
            {
                UnityEngine.Assertions.Assert.IsTrue(
                    _validatingTrigger.Enabled && _validatingTrigger.Value != null,
                    $"{nameof(InputTutorialDisplay)} validation type is set as {nameof(ValidationType.TRIGGER_ENTER)} but no collider is referenced!");

                if (!_validatingTrigger.Value.TryGetComponent(out RSLib.Physics2DEventReceiver physics2DEventReceiver))
                {
                    CProLogger.LogError(this, $"{nameof(InputTutorialDisplay)} collider has no {nameof(RSLib.Physics2DEventReceiver)}!");
                    yield break;
                }
                
                physics2DEventReceiver.TriggerEntered += (collider) => RaiseValidationEvent(ValidationType.TRIGGER_ENTER);
            }
            
            Debug.LogError("Registering move event."); // TODO.
        }
        
        private void Start()
        {
            StartCoroutine(InitEvents());
        }
    }
}
