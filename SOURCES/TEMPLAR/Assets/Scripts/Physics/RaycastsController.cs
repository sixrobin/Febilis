namespace Templar.Physics
{
    using UnityEngine;

    public class RaycastsController
    {
        public class Origins
        {
            public Vector2 BottomLeft { get; set; }
            public Vector2 BottomRight { get; set; }
            public Vector2 TopLeft { get; set; }
            public Vector2 TopRight { get; set; }
        }

        public const float SKIN_WIDTH = 0.005f;
        public const float RAYCASTS_SPACING = 0.1f;

        private BoxCollider2D _boxCollider2D;

        public RaycastsController(BoxCollider2D boxCollider2D)
        {
            _boxCollider2D = boxCollider2D;
            ComputeRaycastsSpacings();
            ComputeRaycastOrigins();
        }

        public int HorizontalRaycastsCount { get; private set; }
        public int VerticalRaycastsCount { get; private set; }
        public float HorizontalRaycastsSpacing { get; private set; }
        public float VerticalRaycastsSpacing { get; private set; }

        public Origins RaycastsOrigins { get; private set; } = new Origins();

        public void ComputeRaycastOrigins()
        {
            Bounds bounds = GetColliderBoundsWithSkinWidth();

            RaycastsOrigins.BottomLeft = bounds.min;
            RaycastsOrigins.TopRight = bounds.max;
            RaycastsOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
            RaycastsOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        }

        private Bounds GetColliderBoundsWithSkinWidth()
        {
            Bounds bounds = _boxCollider2D.bounds;
            bounds.Expand(SKIN_WIDTH * -2f);
            return bounds;
        }

        private void ComputeRaycastsSpacings()
        {
            Vector2 boundsSize = GetColliderBoundsWithSkinWidth().size;

            HorizontalRaycastsCount = Mathf.RoundToInt(boundsSize.y / RAYCASTS_SPACING);
            VerticalRaycastsCount = Mathf.RoundToInt(boundsSize.x / RAYCASTS_SPACING);

            HorizontalRaycastsSpacing = boundsSize.y / (HorizontalRaycastsCount - 1);
            VerticalRaycastsSpacing = boundsSize.x / (VerticalRaycastsCount - 1);
        }
    }
}