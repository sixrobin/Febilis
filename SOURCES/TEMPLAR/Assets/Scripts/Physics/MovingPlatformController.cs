namespace Templar.Physics
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class MovingPlatformController : MonoBehaviour
    {
        private struct PassengerVelocity
        {
            public PassengerVelocity(IPlatformPassenger passenger, Vector3 vel, bool standingOnPlatform, bool moveBeforePlatform)
            {
                Passenger = passenger;
                Velocity = vel;
                StandingOnPlatform = standingOnPlatform;
                MoveBeforePlatform = moveBeforePlatform;
            }
            
            public IPlatformPassenger Passenger { get; private set; }
            public Vector3 Velocity { get; private set; }
            public bool StandingOnPlatform { get; private set; }
            public bool MoveBeforePlatform { get; private set; }
        }

        [SerializeField] private BoxCollider2D _boxCollider2D = null;
        [SerializeField] private LayerMask _passengersMask = 0;
        [SerializeField] private Vector2 _velocity = Vector2.zero;

        private RaycastsController _raycastsCtrl;

        private static System.Collections.Generic.Dictionary<Collider2D, IPlatformPassenger> s_alreadyKnownPassengers = new System.Collections.Generic.Dictionary<Collider2D, IPlatformPassenger>();

        private System.Collections.Generic.HashSet<IPlatformPassenger> _movedPassengers = new System.Collections.Generic.HashSet<IPlatformPassenger>();
        private System.Collections.Generic.List<PassengerVelocity> _passengersVelocities = new System.Collections.Generic.List<PassengerVelocity>();

        private void ComputePassengersVelocity(Vector3 vel)
        {
            _movedPassengers.Clear();
            _passengersVelocities.Clear();

            float signX = Mathf.Sign(vel.x);
            float signY = Mathf.Sign(vel.y);

            if (vel.y != 0f)
            {
                float length = Mathf.Abs(vel.y) + RaycastsController.SKIN_WIDTH;

                for (int i = 0; i < _raycastsCtrl.VerticalRaycastsCount; ++i)
                {
                    Vector2 rayOrigin = (signY == 1f ? _raycastsCtrl.RaycastsOrigins.TopLeft : _raycastsCtrl.RaycastsOrigins.BottomLeft) + Vector2.right * i * _raycastsCtrl.VerticalRaycastsSpacing;
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * signY, length, _passengersMask);

                    if (hit)
                    {
                        if (!s_alreadyKnownPassengers.TryGetValue(hit.collider, out IPlatformPassenger passenger))
                            if (hit.collider.TryGetComponent(out passenger))
                                s_alreadyKnownPassengers.Add(hit.collider, passenger);

                        if (passenger == null || _movedPassengers.Contains(passenger))
                            continue;

                        _movedPassengers.Add(passenger);

                        float pushX = signY == 1f ? vel.x : 0f;
                        float pushY = vel.y - (hit.distance - RaycastsController.SKIN_WIDTH) * signY;
                        _passengersVelocities.Add(new PassengerVelocity(passenger, new Vector3(pushX, pushY), signY == 1f, true));
                    }
                }
            }

            if (vel.x != 0f)
            {
                float length = Mathf.Abs(vel.x) + RaycastsController.SKIN_WIDTH;

                for (int i = 0; i < _raycastsCtrl.HorizontalRaycastsCount; ++i)
                {
                    Vector2 rayOrigin = (signX == 1f ? _raycastsCtrl.RaycastsOrigins.BottomRight : _raycastsCtrl.RaycastsOrigins.BottomLeft) + Vector2.up * i * _raycastsCtrl.HorizontalRaycastsSpacing;
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * signX, length, _passengersMask);

                    if (hit)
                    {
                        if (!s_alreadyKnownPassengers.TryGetValue(hit.collider, out IPlatformPassenger passenger))
                            if (hit.collider.TryGetComponent(out passenger))
                                s_alreadyKnownPassengers.Add(hit.collider, passenger);

                        if (passenger == null || _movedPassengers.Contains(passenger))
                            continue;

                        _movedPassengers.Add(passenger);

                        float pushX = vel.x - (hit.distance - RaycastsController.SKIN_WIDTH) * signX;
                        _passengersVelocities.Add(new PassengerVelocity(passenger, new Vector3(pushX, 0f), false, true));
                    }
                }
            }

            // Passenger on horizontally or downward moving platform.
            if (signY == -1 || vel.y == 0f && vel.x != 0f)
            {
                float length = RaycastsController.SKIN_WIDTH * 2;

                for (int i = 0; i < _raycastsCtrl.VerticalRaycastsCount; ++i)
                {
                    Vector2 rayOrigin = _raycastsCtrl.RaycastsOrigins.TopLeft + Vector2.right * i * _raycastsCtrl.VerticalRaycastsSpacing;
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, length, _passengersMask);

                    if (hit)
                    {
                        if (!s_alreadyKnownPassengers.TryGetValue(hit.collider, out IPlatformPassenger passenger))
                            if (hit.collider.TryGetComponent(out passenger))
                                s_alreadyKnownPassengers.Add(hit.collider, passenger);

                        if (passenger == null || _movedPassengers.Contains(passenger))
                            continue;

                        _movedPassengers.Add(passenger);
                        _passengersVelocities.Add(new PassengerVelocity(passenger, vel, true, false));
                    }
                }
            }
        }

        private void ApplyPassengersVelocity(bool moveBeforePlatform)
        {
            foreach (PassengerVelocity passengerVel in _passengersVelocities)
                if (passengerVel.MoveBeforePlatform == moveBeforePlatform)
                    passengerVel.Passenger.OnPlatformMoved(passengerVel.Velocity, passengerVel.StandingOnPlatform);
        }

        private void Awake()
        {
            _raycastsCtrl = new RaycastsController(_boxCollider2D);
        }

        private void Update()
        {
            _raycastsCtrl.ComputeRaycastOrigins();

            Vector3 vel = _velocity;

            ComputePassengersVelocity(vel);

            ApplyPassengersVelocity(true);
            transform.Translate(vel * Time.deltaTime);
            ApplyPassengersVelocity(false);
        }
    }
}