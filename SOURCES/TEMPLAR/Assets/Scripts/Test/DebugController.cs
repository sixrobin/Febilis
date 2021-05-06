namespace Templar.Tmp
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class DebugController : MonoBehaviour
    {
        public class Snapshot
        {
            public Snapshot(Vector3 pos, Vector3 vel)
            {
                Pos = pos;
                Vel = vel;
            }

            public Vector3 Pos { get; private set; }
            public Vector3 Vel { get; private set; }
        }

        [SerializeField] private BoxCollider2D _box = null;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpHeight = 3f;
        [SerializeField] private float _jumpApexDur = 0.5f;
        [SerializeField] private LayerMask _mask = 0;
        [SerializeField] private Slider _snapshotSlider = null;

        private Vector3 _currVel;

        private List<Snapshot> _snapshots = new List<Snapshot>();
        private int _currSnapshotIndex = 0;
        private bool _snapshotMode = false;

        public Templar.Physics.CollisionsController CollisionsCtrl { get; protected set; }
        public float Gravity { get; private set; }
        public float JumpVel { get; private set; }

        private void OnSliderValueChanged(float value)
        {
            _currSnapshotIndex = Mathf.RoundToInt(RSLib.Maths.Maths.Normalize(value, 0f, 1f, _snapshots.Count - 1, 0));
        }

        private void Move()
        {
            CollisionsCtrl.BackupCurrentState();

            if (CollisionsCtrl.Vertical)
                _currVel.y = 0f;

            float hor = Input.GetAxisRaw("Horizontal");
            _currVel.x = _moveSpeed * hor;

            if (Input.GetKeyDown(KeyCode.Space))
                _currVel.y = JumpVel;

            _currVel.y += Gravity * Time.deltaTime;

            CollisionsCtrl.ComputeRaycastOrigins();
            CollisionsCtrl.CurrentStates.Reset();

            Vector3 vel = _currVel * Time.deltaTime;

            if (vel.x != 0f)
            {
                CollisionsCtrl.ComputeHorizontalCollisions(ref vel, false);
                transform.Translate(new Vector3(vel.x, 0f));
                CollisionsCtrl.ComputeRaycastOrigins();
            }

            if (vel.y != 0f)
            {
                CollisionsCtrl.ComputeVerticalCollisions(ref vel, false);
                transform.Translate(new Vector3(0f, vel.y));
            }
        }

        private void RecordSnapshot()
        {
            _snapshots.Insert(0, new Snapshot(transform.position, _currVel));
        }

        private void VisualizeSnapshot()
        {
            Snapshot snapshot = _snapshots[_currSnapshotIndex];

            transform.position = snapshot.Pos;

            Vector3 vel = snapshot.Vel;

            CollisionsCtrl.ComputeCollisions(ref vel);
        }

        private void Awake()
        {
            CollisionsCtrl = new Templar.Physics.CollisionsController(_box, _mask);
            CollisionsCtrl.Ground(transform);

            Gravity = -(2f * _jumpHeight) / (_jumpApexDur * _jumpApexDur);
            JumpVel = Mathf.Abs(Gravity) * _jumpApexDur;

            _snapshotSlider.onValueChanged.AddListener(OnSliderValueChanged);
            _snapshotSlider.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _snapshotMode = !_snapshotMode;
                _snapshotSlider.gameObject.SetActive(_snapshotMode);
            }

            if (!_snapshotMode)
            {
                Move();
                RecordSnapshot();
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                _currSnapshotIndex = RSLib.Helpers.Mod(--_currSnapshotIndex, _snapshots.Count);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                _currSnapshotIndex = ++_currSnapshotIndex % _snapshots.Count;

            VisualizeSnapshot();
        }
    }
}