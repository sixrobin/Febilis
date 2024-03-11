namespace Templar.Attack
{
    using UnityEngine;

    public class HittablePickup : MonoBehaviour, IHittable
    {
        [SerializeField] private UnityEngine.Events.UnityEvent _onHit = null;

        public delegate void HitEventArgs();
        public event HitEventArgs Hit;

        public bool SpawnVFXOnHit => true;
        
        public HitLayer HitLayer => HitLayer.PICKUP;

        public bool Interactable { get; set; }

        public bool CanBeHit(HitInfos hitInfos)
        {
            return Interactable;
        }

        public void OnHit(HitInfos hitInfos)
        {
            Hit?.Invoke();
            _onHit?.Invoke();
        }
    }
}