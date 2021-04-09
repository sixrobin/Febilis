namespace Templar.Physics
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CollisionsController : RaycastsController
    {
        public class CollisionsStates
        {
            private System.Collections.Generic.Dictionary<CollisionOrigin, bool> _states;

            public CollisionsStates()
            {
                _states = new System.Collections.Generic.Dictionary<CollisionOrigin, bool>(new RSLib.Framework.Comparers.EnumComparer<CollisionOrigin>())
                {
                    { CollisionOrigin.ABOVE, false },
                    { CollisionOrigin.BELOW, false },
                    { CollisionOrigin.LEFT, false },
                    { CollisionOrigin.RIGHT, false }
                };
            }

            public void Reset()
            {
                _states[CollisionOrigin.ABOVE] = false;
                _states[CollisionOrigin.BELOW] = false;
                _states[CollisionOrigin.LEFT] = false;
                _states[CollisionOrigin.RIGHT] = false;
            }

            public void Copy(CollisionsStates states)
            {
                _states[CollisionOrigin.ABOVE] = states.GetCollisionState(CollisionOrigin.ABOVE);
                _states[CollisionOrigin.BELOW] = states.GetCollisionState(CollisionOrigin.BELOW);
                _states[CollisionOrigin.LEFT] = states.GetCollisionState(CollisionOrigin.LEFT);
                _states[CollisionOrigin.RIGHT] = states.GetCollisionState(CollisionOrigin.RIGHT);
            }

            public void SetCollision(CollisionOrigin origin, bool state = true)
            {
                UnityEngine.Assertions.Assert.IsTrue(origin != CollisionOrigin.NONE);
                _states[origin] = state;
            }

            public bool GetCollisionState(CollisionOrigin origin)
            {
                UnityEngine.Assertions.Assert.IsTrue(origin != CollisionOrigin.NONE);
                return _states[origin];
            }

            public bool GetHorizontalCollisionsState()
            {
                return _states[CollisionOrigin.RIGHT] || _states[CollisionOrigin.LEFT];
            }

            public bool GetVerticalCollisionsState()
            {
                return _states[CollisionOrigin.ABOVE] || _states[CollisionOrigin.BELOW];
            }

            public bool GetAnyCollisionsState()
            {
                return GetHorizontalCollisionsState() || GetVerticalCollisionsState();
            }

            public override string ToString()
            {
                string collisionsStr = string.Empty;

                if (_states[CollisionOrigin.ABOVE])
                    collisionsStr += "ABOVE | ";
                if (_states[CollisionOrigin.BELOW])
                    collisionsStr += "BELOW | ";
                if (_states[CollisionOrigin.LEFT])
                    collisionsStr += "LEFT | ";
                if (_states[CollisionOrigin.RIGHT])
                    collisionsStr += "RIGHT | ";

                collisionsStr = collisionsStr.RemoveLast(3);
                return collisionsStr;
            }
        }

        public class CollisionInfos
        {
            public CollisionInfos(CollisionOrigin origin, RaycastHit2D hit)
            {
                Origin = origin;
                Hit = hit;
            }

            public CollisionOrigin Origin { get; private set; }
            public RaycastHit2D Hit { get; private set; }
        }

        private LayerMask _collisionMask;

        // Used to trigger events manually after movement has been applied.
        private System.Collections.Generic.Queue<CollisionInfos> _detectedCollisionsForEvent = new System.Collections.Generic.Queue<CollisionInfos>();

        private static System.Collections.Generic.Dictionary<Collider2D, SideTriggerOverrider> s_sharedKnownSideTriggerOverriders
            = new System.Collections.Generic.Dictionary<Collider2D, SideTriggerOverrider>();

        public CollisionsController(BoxCollider2D boxCollider2D, LayerMask collisionMask) : base(boxCollider2D)
        {
            _collisionMask = collisionMask;
        }

        public delegate void CollisionDetectedEventHandler(CollisionInfos collisionInfos);
        public event CollisionDetectedEventHandler CollisionDetected;

        public enum CollisionOrigin
        {
            NONE,
            ABOVE,
            BELOW,
            LEFT,
            RIGHT
        }

        public CollisionsStates CurrentStates { get; private set; } = new CollisionsStates();
        public CollisionsStates PreviousStates { get; private set; } = new CollisionsStates();

        public bool Above => CurrentStates.GetCollisionState(CollisionOrigin.ABOVE);
        public bool Below => CurrentStates.GetCollisionState(CollisionOrigin.BELOW);
        public bool Left => CurrentStates.GetCollisionState(CollisionOrigin.LEFT);
        public bool Right => CurrentStates.GetCollisionState(CollisionOrigin.RIGHT);
        public bool Horizontal => CurrentStates.GetHorizontalCollisionsState();
        public bool Vertical => CurrentStates.GetVerticalCollisionsState();
        public bool Any => CurrentStates.GetAnyCollisionsState();

        public virtual LayerMask ComputeCollisionMask()
        {
            // Can be overriden to suit any collisions actor needs.
            return _collisionMask;
        }

        public void TriggerDetectedCollisionsEvents()
        {
            while (_detectedCollisionsForEvent.Count > 0)
            {
                // Need to do this in two lines, else loop will be infinite if event has no listener.
                CollisionInfos collision = _detectedCollisionsForEvent.Dequeue();
                CollisionDetected?.Invoke(collision);
            }
        }

        public void Ground(Transform transform, bool triggerEvent = false)
        {
            for (int i = 0; i < VerticalRaycastsCount; ++i)
            {
                Vector2 rayOrigin = RaycastsOrigins.BottomLeft + Vector2.right * i * VerticalRaycastsSpacing;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, _collisionMask);

                if (hit)
                {
                    CurrentStates.SetCollision(CollisionOrigin.BELOW);
                    if (triggerEvent)
                        CollisionDetected?.Invoke(new CollisionInfos(CollisionOrigin.BELOW, hit));

                    transform.Translate(new Vector3(0f, -hit.distance + SKIN_WIDTH));
                    CProLogger.Log(this, $"Ground on {hit.transform.name} (new position : x={transform.position.x}/y={transform.position.y}).", hit.collider.gameObject);
                    return;
                }
            }

            CProLogger.LogWarning(this, $"No ground has been found to ground {transform.name}.");
        }

        public void ComputeCollisions(ref Vector3 vel)
        {
            vel = ComputeCollisions(vel);
        }

        public Vector3 ComputeCollisions(Vector3 vel)
        {
            ComputeRaycastOrigins();
            CurrentStates.Reset();

            if (vel.x != 0f)
                ComputeHorizontalCollisions(ref vel);

            if (vel.y != 0f)
                ComputeVerticalCollisions(ref vel);

            return vel;
        }

        public void ComputeHorizontalCollisions(ref Vector3 vel)
        {
            float sign = Mathf.Sign(vel.x);
            float length = vel.x * sign + SKIN_WIDTH;

            for (int i = 0; i < HorizontalRaycastsCount; ++i)
            {
                Vector2 rayOrigin = (sign == 1f ? RaycastsOrigins.BottomRight : RaycastsOrigins.BottomLeft) + Vector2.up * i * HorizontalRaycastsSpacing;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * sign, length, ComputeCollisionMask());

                if (hit)
                {
                    if (!hit.collider.isTrigger)
                    {
                        if (!s_sharedKnownSideTriggerOverriders.TryGetValue(hit.collider, out SideTriggerOverrider sideTriggerOverrider))
                            if (hit.collider.TryGetComponent(out sideTriggerOverrider))
                                s_sharedKnownSideTriggerOverriders.Add(hit.collider, sideTriggerOverrider);

                        if (sideTriggerOverrider?.IsSideSetAsTrigger(sign == 1f ? CollisionOrigin.LEFT : CollisionOrigin.RIGHT) ?? false)
                            continue;
                    }
                    else
                    {
                        continue;
                    }

                    Debug.DrawRay(rayOrigin, Vector2.right * sign, Color.red);

                    if (hit.distance <= Mathf.Epsilon)
                        continue;

                    length = hit.distance;
                    vel.x = (length - SKIN_WIDTH) * sign;

                    CurrentStates.SetCollision(CollisionOrigin.LEFT, sign == -1f);
                    CurrentStates.SetCollision(CollisionOrigin.RIGHT, sign == 1f);
                    RegisterCollisionForEvent(new CollisionInfos(CurrentStates.GetCollisionState(CollisionOrigin.LEFT) ? CollisionOrigin.LEFT : CollisionOrigin.RIGHT, hit));

                    return;
                }
                else
                {
                    Debug.DrawRay(rayOrigin, Vector2.right * sign, Color.yellow);
                }
            }
        }

        public void ComputeVerticalCollisions(ref Vector3 vel)
        {
            float sign = Mathf.Sign(vel.y);
            float length = vel.y * sign + SKIN_WIDTH;

            for (int i = 0; i < VerticalRaycastsCount; ++i)
            {
                Vector2 rayOrigin = (sign == 1f ? RaycastsOrigins.TopLeft : RaycastsOrigins.BottomLeft) + Vector2.right * i * VerticalRaycastsSpacing;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * sign, length, ComputeCollisionMask());

                if (hit)
                {
                    if (!hit.collider.isTrigger)
                    {
                        if (!s_sharedKnownSideTriggerOverriders.TryGetValue(hit.collider, out SideTriggerOverrider sideTriggerOverrider))
                            if (hit.collider.TryGetComponent(out sideTriggerOverrider))
                                s_sharedKnownSideTriggerOverriders.Add(hit.collider, sideTriggerOverrider);

                        if (sideTriggerOverrider?.IsSideSetAsTrigger(sign == 1f ? CollisionOrigin.BELOW : CollisionOrigin.ABOVE) ?? false)
                            continue;
                    }
                    else
                    {
                        continue;
                    }

                    Debug.DrawRay(rayOrigin, Vector2.up * sign, Color.red);

                    if (hit.distance <= Mathf.Epsilon)
                        continue;

                    length = hit.distance;
                    vel.y = (length - SKIN_WIDTH) * sign;

                    CurrentStates.SetCollision(CollisionOrigin.ABOVE, sign == 1f);
                    CurrentStates.SetCollision(CollisionOrigin.BELOW, sign == -1f);
                    RegisterCollisionForEvent(new CollisionInfos(CurrentStates.GetCollisionState(CollisionOrigin.ABOVE) ? CollisionOrigin.ABOVE : CollisionOrigin.BELOW, hit));

                    return;
                }
                else
                {
                    Debug.DrawRay(rayOrigin, Vector2.up * sign, Color.yellow);
                }
            }
        }

        public void BackupCurrentState()
        {
            PreviousStates.Copy(CurrentStates);
        }

        private void RegisterCollisionForEvent(CollisionInfos collisionInfos)
        {
            // [TODO] CollisionInfos pooling ?
            _detectedCollisionsForEvent.Enqueue(collisionInfos);
        }
    }
}