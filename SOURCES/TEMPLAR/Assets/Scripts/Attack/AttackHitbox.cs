namespace Templar.Attack
{
    using UnityEngine;

    public class AttackHitbox : MonoBehaviour
    {
        public class HitEventArgs : System.EventArgs
        {
            public HitEventArgs(float dir)
            {
                Dir = dir;
            }

            public float Dir { get; private set; }
        }

        [SerializeField] private string _id = string.Empty;

        private static System.Collections.Generic.Dictionary<Collider2D, IHittable> s_sharedKnownHittables = new System.Collections.Generic.Dictionary<Collider2D, IHittable>();

        private System.Collections.Generic.List<IHittable> _hitThisTime = new System.Collections.Generic.List<IHittable>();
        private Datas.AttackDatas _attackDatas;

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

        public void Trigger(float dir, Datas.AttackDatas attackDatas)
        {
            UnityEngine.Assertions.Assert.IsNull(_hitCoroutine, $"Triggering hit on {transform.name} hitbox that seems to already run a coroutine.");

            _attackDatas = attackDatas;
            Dir = dir;

            _hitCoroutine = HitCoroutine(_attackDatas.HitDur);
            StartCoroutine(_hitCoroutine);
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_attackDatas, "Hitbox triggered something even though attack datas have not been set.");

            if (!s_sharedKnownHittables.TryGetValue(collision, out IHittable hittable))
                if (collision.TryGetComponent(out hittable))
                    s_sharedKnownHittables.Add(collision, hittable);

            if (hittable == null || !hittable.CanBeHit || (_attackDatas.HitLayer & hittable.HitLayer) == 0 || _hitThisTime.Contains(hittable))
                return;

            _hitThisTime.Add(hittable);
            hittable.OnHit(new HitInfos(_attackDatas, Dir, _source)); // [TODO] HitDatas pooling ?
            Hit(new HitEventArgs(Dir)); // [TODO] HitEventArgs pooling ?
        }
    }
}