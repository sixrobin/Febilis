namespace RSLib.Extensions
{
    using UnityEngine;

    public static class RectTransformExtensions
    {
        #region CLAMP
        
        /// <summary>
        /// Clamps the RectTransform position to another RectTransform position.
        /// </summary>
        /// <param name="rectTransform">RectTransform to clamp.</param>
        /// <param name="otherRectTransform">Reference RectTransform to clamp to.</param>
        public static void ClampTo(this RectTransform rectTransform, RectTransform otherRectTransform)
        {
            Vector3 localPos = rectTransform.localPosition;

            Rect rect = rectTransform.rect;
            Rect otherRect = otherRectTransform.rect;

            Vector3 minPos = otherRect.min - rect.min;
            Vector3 maxPos = otherRect.max - rect.max;

            localPos.x = Mathf.Clamp(rectTransform.localPosition.x, minPos.x, maxPos.x);
            localPos.y = Mathf.Clamp(rectTransform.localPosition.y, minPos.y, maxPos.y);

            rectTransform.localPosition = localPos;
        }

        /// <summary>
        /// Clamps the RectTransform position to its parent RectTransform.
        /// </summary>
        public static void ClampToParent(this RectTransform rectTransform)
        {
            Transform parent = rectTransform.parent;
            
            UnityEngine.Assertions.Assert.IsNotNull(
                parent, 
                $"Cannot clamp {rectTransform.name} RectTransform to its parent since it has no parent.");

            RectTransform parentRectTransform = parent.GetComponent<RectTransform>();

            UnityEngine.Assertions.Assert.IsNotNull(
                parentRectTransform,
                $"{rectTransform.name} RectTransform parent's {parentRectTransform.name} has no RectTransform component to do a clamp.");

            rectTransform.ClampTo(parentRectTransform);
        }
        
        #endregion // CLAMP
        
        #region GENERAL

        /// <summary>
        /// Computes the RectTransform rect as world coordinates.
        /// </summary>
        /// <returns>RectTransform world rect.</returns>
        public static Rect GetWorldRect(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3 bottomLeftPosition = corners[0];
            
            Vector3 lossyScale = rectTransform.lossyScale;
            Rect rect = rectTransform.rect;
            Vector2 size = new Vector2(lossyScale.x * rect.size.x, lossyScale.y * rect.size.y);
            
            return new Rect(bottomLeftPosition, size);
        }

        #endregion // GENERAL

        #region PIVOT & ANCHORS

        /// <summary>
        /// Sets the RectTransform pivot without changing its position.
        /// </summary>
        /// <param name="rectTransform">RectTransform to change the pivot of.</param>
        /// <param name="pivot">New pivot.</param>
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }
        
        /// <summary>
        /// Sets the RectTransform min anchor without changing its position.
        /// </summary>
        /// <param name="rectTransform">RectTransform to change the min anchor of.</param>
        /// <param name="anchorMin">New min anchor.</param>
        public static void SetAnchorMin(this RectTransform rectTransform, Vector2 anchorMin)
        {
            Vector3 localPosition = rectTransform.localPosition;        
            rectTransform.anchorMin = anchorMin;
            rectTransform.localPosition = localPosition;
        }

        /// <summary>
        /// Sets the RectTransform max anchor without changing its position.
        /// </summary>
        /// <param name="rectTransform">RectTransform to change the max anchor of.</param>
        /// <param name="anchorMax">New max anchor.</param>
        public static void SetAnchorMax(this RectTransform rectTransform, Vector2 anchorMax)
        {
            Vector3 localPosition = rectTransform.localPosition;
            rectTransform.anchorMax = anchorMax;
            rectTransform.localPosition = localPosition;
        }

        /// <summary>
        /// Sets the RectTransform anchors without changing its position.
        /// </summary>
        /// <param name="rectTransform">RectTransform to change the pivot of.</param>
        /// <param name="anchors">New anchors (both min and max).</param>
        public static void SetAnchors(this RectTransform rectTransform, Vector2 anchors)
        {
            Vector3 localPosition = rectTransform.localPosition;        
            rectTransform.anchorMin = anchors;
            rectTransform.anchorMax = anchors;
            rectTransform.localPosition = localPosition;
        }

        #endregion // PIVOT & ANCHORS
    }
}