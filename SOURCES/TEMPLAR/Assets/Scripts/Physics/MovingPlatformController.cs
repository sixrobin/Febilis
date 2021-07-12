namespace Templar.Physics
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class MovingPlatformController : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _boxCollider2D = null;
        [SerializeField] private LayerMask _passengersMask = 0;
        [SerializeField] private Vector2 _velocity = Vector2.zero;

        private RaycastsController _raycastsCtrl;

        private System.Collections.Generic.HashSet<Transform> _movedPassengers = new System.Collections.Generic.HashSet<Transform>();

        private void MovePassengers(Vector3 vel)
        {
            _movedPassengers.Clear();

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
                        if (_movedPassengers.Contains(hit.transform))
                            continue;

                        _movedPassengers.Add(hit.transform);

                        float pushX = signY == 1f ? vel.x : 0f;
                        float pushY = vel.y - (hit.distance - RaycastsController.SKIN_WIDTH) * signY;
                        hit.transform.Translate(new Vector3(pushX, pushY));
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
                        if (_movedPassengers.Contains(hit.transform))
                            continue;

                        _movedPassengers.Add(hit.transform);

                        float pushX = vel.x - (hit.distance - RaycastsController.SKIN_WIDTH) * signX;
                        hit.transform.Translate(new Vector3(pushX, 0f));
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
                        if (_movedPassengers.Contains(hit.transform))
                            continue;

                        _movedPassengers.Add(hit.transform);
                        hit.transform.Translate(vel);
                    }
                }
            }
        }

        private void Awake()
        {
            _raycastsCtrl = new RaycastsController(_boxCollider2D);
        }

        private void Update()
        {
            _raycastsCtrl.ComputeRaycastOrigins();

            Vector3 vel = _velocity * Time.deltaTime;

            MovePassengers(vel);
            transform.Translate(vel);
        }
    }
}