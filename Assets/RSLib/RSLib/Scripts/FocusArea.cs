namespace RSLib
{
    using UnityEngine;

    /// <summary>
    /// Represents a rectangular area to follow a BoxCollider2D bounds position.
    /// Whenever one of the box' bound goes out of the area, the area follows
    /// the box so that it stays inside of the area.
    /// </summary>
    public class FocusArea
    {
        private float _left;
        private float _right;
        private float _top;
        private float _bottom;

        private BoxCollider2D _targetBox = null;

        public FocusArea(BoxCollider2D targetBox, Vector2 size)
        {
            _targetBox = targetBox;
            SetSize(size);
        }

        public Vector2 Center { get; private set; }
        public Vector2 Size { get; private set; }
        public Vector2 Velocity { get; private set; }

        /// <summary>
        /// Sets the area size and refreshes the values according to new size.
        /// </summary>
        /// <param name="size">New area size, must be larger than target box size.</param>
        public void SetSize(Vector2 size)
        {
            UnityEngine.Assertions.Assert.IsTrue(size.x >= _targetBox.bounds.size.x, "Focus area size must have a larger x value than target bounds x value.");
            UnityEngine.Assertions.Assert.IsTrue(size.y >= _targetBox.bounds.size.y, "Focus area size must have a larger y value than target bounds y value.");

            Size = size;
            RefreshValues();
        }

        /// <summary>
        /// Updates the area's position according to target box bounds positions.
        /// Also updates the velocity value, representing the area movement amplitude if it needed to move.
        /// </summary>
        public void Update()
        {
            float shiftX = 0f;
            float shiftY = 0f;

            if (_targetBox.bounds.min.x < _left)
                shiftX = _targetBox.bounds.min.x - _left;
            else if (_targetBox.bounds.max.x > _right)
                shiftX = _targetBox.bounds.max.x - _right;

            if (_targetBox.bounds.min.y < _bottom)
                shiftY = _targetBox.bounds.min.y - _bottom;
            else if (_targetBox.bounds.max.y > _top)
                shiftY = _targetBox.bounds.max.y - _top;

            _left += shiftX;
            _right += shiftX;
            _top += shiftY;
            _bottom += shiftY;

            UpdateCenter();
            Velocity = new Vector2(shiftX, shiftY);
        }

        private void RefreshValues()
        {
            Bounds bounds = _targetBox.bounds;
            
            _left = bounds.center.x - Size.x * 0.5f;
            _right = bounds.center.x + Size.x * 0.5f;
            _bottom = bounds.min.y;
            _top = bounds.min.y + Size.y;

            UpdateCenter();
        }

        private void UpdateCenter()
        {
            Center = new Vector2(_left + _right, _top + _bottom) * 0.5f;
        }

        /// <summary>
        /// Draws the area rect.
        /// Must be called inside OnDrawGizmos or OnDrawGizmosSelected.
        /// </summary>
        /// <param name="color">Rect color.</param>
        public void DrawArea(Color color)
        {
#if UNITY_EDITOR
            Gizmos.color = color;
            Gizmos.DrawCube(Center, Size);
#endif
        }
    }
}