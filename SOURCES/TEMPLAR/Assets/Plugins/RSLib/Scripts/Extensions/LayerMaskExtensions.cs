namespace RSLib.Extensions
{
    using UnityEngine;

    public static class LayerMaskExtensions
    {
        #region GENERAL

        /// <summary>Checks if a layer mask has a given layer enabled.</summary>
        /// <param name="layer">Layer to check.</param>
        /// <returns>True if layer is enabled in the mask, else false.</returns>
        public static bool HasLayer(this LayerMask layerMask, int layer)
        {
            return layerMask.value == (layerMask.value | 1 << layer);
        }

        /// <summary>Checks if a layer mask has a given layer enabled.</summary>
        /// <param name="layer">Layer to check.</param>
        /// <returns>True if layer is enabled in the mask, else false.</returns>
        public static bool HasLayer(this LayerMask layerMask, string layer)
        {
            if (string.IsNullOrEmpty(layer))
                return false;

            return layerMask.value == (layerMask.value | 1 << LayerMask.NameToLayer(layer));
        }

        #endregion GENERAL
    }
}