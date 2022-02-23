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
            INVENTORY,
            JUMP
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

            switch (_validationType)
            {
                case ValidationType.TRIGGER_ENTER:
                case ValidationType.ROLL when _validatingTrigger.Enabled:
                case ValidationType.JUMP when _validatingTrigger.Enabled:
                    UnityEngine.Assertions.Assert.IsTrue(
                        _validatingTrigger.Enabled && _validatingTrigger.Value != null,
                        $"{nameof(InputTutorialDisplay)} validation type is set as {nameof(ValidationType.TRIGGER_ENTER)} but no collider is referenced and/or enabled!");

                    if (!_validatingTrigger.Value.TryGetComponent(out RSLib.Physics2DEventReceiver physics2DEventReceiver))
                    {
                        CProLogger.LogError(this, $"{nameof(InputTutorialDisplay)} collider has no {nameof(RSLib.Physics2DEventReceiver)}!");
                        yield break;
                    }
                    
                    physics2DEventReceiver.TriggerEntered += (collider) => RaiseValidationEvent(_validationType);
                    break;
                
                case ValidationType.ROLL when !_validatingTrigger.Enabled:
                    playerCtrl.RollCtrl.Rolled += () => RaiseValidationEvent(ValidationType.ROLL);
                    break;
                
                case ValidationType.JUMP when !_validatingTrigger.Enabled:
                    playerCtrl.JumpCtrl.Jumped += () => RaiseValidationEvent(ValidationType.JUMP);
                    break;
                                
                case ValidationType.HEAL:
                    playerCtrl.HealthCtrl.UnitHealthChanged += (args) =>
                    {
                        if (!args.IsLoss)
                            RaiseValidationEvent(ValidationType.HEAL);
                    };
                    break;
                
                case ValidationType.INVENTORY:
                    Manager.GameManager.InventoryView.DisplayChanged += (displayed) =>
                    {
                        if (displayed)
                            RaiseValidationEvent(ValidationType.INVENTORY);
                    };
                    break;

                case ValidationType.ATTACK:
                    playerCtrl.AttackCtrl.AttackHitTriggered += () => RaiseValidationEvent(ValidationType.ATTACK);
                    break;
                
                case ValidationType.EFFECTOR:
                    playerCtrl.CollisionsCtrl.EffectorDown += (effector) => RaiseValidationEvent(ValidationType.EFFECTOR);
                    break;
                
                case ValidationType.INTERACTION:
                    playerCtrl.Interacter.Interacted += (interactable) => RaiseValidationEvent(ValidationType.INTERACTION);
                    break;
            }
            
            Debug.LogError("Registering move event."); // TODO.
        }
        
        private void Start()
        {
            StartCoroutine(InitEvents());
        }
    }
}
