namespace Templar.Attack
{
    using UnityEngine;

    public class AttackHitbox : MonoBehaviour
    {
        public class HitEventArgs : System.EventArgs
        {
            public HitEventArgs(IHittable hittable, float dir)
            {
                Hittable = hittable;
                Dir = dir;
            }

            public IHittable Hittable { get; private set; }
            public float Dir { get; private set; }
        }

        [SerializeField] private string _id = string.Empty;

        private static System.Collections.Generic.Dictionary<Collider2D, IHittable> s_sharedKnownHittables
            = new System.Collections.Generic.Dictionary<Collider2D, IHittable>();

        private System.Collections.Generic.List<IHittable> _hitThisTime = new System.Collections.Generic.List<IHittable>();
        private Datas.Attack.AttackDatas _attackDatas;

        private Collider2D _hitbox;
        private Transform _source;
        private System.Collections.IEnumerator _hitCoroutine;

        public delegate void HitEventHandler(HitEventArgs hitArgs);
        public event HitEventHandler Hit;

        public string Id => _id;
        public float Dir { get; private set; }

        public void SetAttackSource(Transform source)
        {
            _source = source;
        }

        public void Trigger(float dir, Datas.Attack.AttackDatas attackDatas)
        {
            UnityEngine.Assertions.Assert.IsNull(_hitCoroutine, $"Triggering hit on {transform.name} hitbox that seems to already run a coroutine.");

            Dir = dir;
            _attackDatas = attackDatas;
            StartCoroutine(_hitCoroutine = HitCoroutine(_attackDatas.HitDur));
        }

        private System.Collections.IEnumerator HitCoroutine(float dur)
        {
            _hitbox.enabled = true;
            yield return RSLib.Yield.SharedYields.WaitForSeconds(dur);

            _hitThisTime.Clear();
            _hitbox.enabled = false;
            _hitCoroutine = null;
        }

        private void Awake()
        {
            _hitbox = GetComponent<Collider2D>();
            UnityEngine.Assertions.Assert.IsNotNull(_hitbox, $"No collider found on attack hitbox {transform.name}.");
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_attackDatas, "Hitbox triggered something even though attack datas have not been set.");

            Templar.Physics.Triggerables.TriggerableObject.SharedTriggerablesByColliders.TryGetValue(collider, out Templar.Physics.Triggerables.TriggerableObject triggerable);
            triggerable?.TryTrigger(Templar.Physics.Triggerables.TriggerableSourceType.ATTACK);

            if (!s_sharedKnownHittables.TryGetValue(collider, out IHittable hittable))
                if (collider.TryGetComponent(out hittable))
                    s_sharedKnownHittables.Add(collider, hittable);

            HitInfos hitInfos = new HitInfos(_attackDatas, Dir, _source);

            if (hittable == null || !hittable.CanBeHit(hitInfos) || !_attackDatas.HitLayer.HasFlag(hittable.HitLayer) || _hitThisTime.Contains(hittable))
                return;

            _hitThisTime.Add(hittable);
            hittable.OnHit(hitInfos);
            Hit(new HitEventArgs(hittable, Dir));
        }
    }
}