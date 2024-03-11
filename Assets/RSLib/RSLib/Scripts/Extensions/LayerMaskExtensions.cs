namespace RSLib.Extensions
{
    using UnityEngine;

    public static class LayerMaskExtensions
    {
        #region GENERAL

        /// <summary>
        /// Checks if a layer mask has a given layer enabled.
        /// </summary>
        /// <param name="layerMask">LayerMask to check.</param>
        /// <param name="layer">Layer to look for.</param>
        /// <returns>True if layer is enabled in the mask, else false.</returns>
        public static bool HasLayer(this LayerMask layerMask, int layer)
        {
            return layerMask.value == (layerMask.value | 1 << layer);
        }

        /// <summary>
        /// Checks if a layer mask has a given layer enabled.
        /// </summary>
        /// <param name="layerMask">LayerMask to check.</param>
        /// <param name="layer">Layer to look for.</param>
        /// <returns>True if layer is enabled in the mask, else false.</returns>
        public static bool HasLayer(this LayerMask layerMask, string layer)
        {
            return !string.IsNullOrEmpty(layer) && layerMask.value == (layerMask.value | 1 << LayerMask.NameToLayer(layer));
        }

        /// <summary>
        /// Gathers all flags names in the layer mask to a string array.
        /// </summary>
        /// <returns>Array containing all flags names.</returns>
        public static string[] MaskToNames(this LayerMask layerMask)
        {
            System.Collections.Generic.List<string> output = new System.Collections.Generic.List<string>();

            for (int i = 0; i < 32; ++i)
            {
                int shifted = 1 << i;
                if ((layerMask & shifted) == shifted)
                {
                    string layerName = LayerMask.LayerToName(i);
                    if (!string.IsNullOrEmpty(layerName))
                        output.Add(layerName);
                }
            }

            return output.ToArray();
        }

        /// <summary>
        /// Returns a string with joined mask flags, separated by a given separator string.
        /// </summary>
        /// <param name="layerMask">LayerMask to display as a string.</param>
        /// <param name="separator">String used to split mask flags.</param>
        /// <returns>String with joined mask flags.</returns>
        public static string MaskToString(this LayerMask layerMask, string separator = ", ")
        {
            return string.Join(separator, MaskToNames(layerMask));
        }

        #endregion // GENERAL

        #region MASK MANIPULATION

        /// <summary>
        /// Adds layers to a layer mask.
        /// </summary>
        /// <param name="layerNames">Layer names to add.</param>
        /// <returns>Layer mask with added layers.</returns>
        public static LayerMask AddLayers(this LayerMask layerMask, params string[] layerNames)
        {
            return layerMask | NamesToMask(layerNames);
        }

        /// <summary>
        /// Adds layers to a layer mask.
        /// </summary>
        /// <param name="layerIndexes">Layer indexes to add.</param>
        /// <returns>Layer mask with added layers.</returns>
        public static LayerMask AddLayers(this LayerMask layerMask, params int[] layerIndexes)
        {
            return layerMask | IndexesToMask(layerIndexes);
        }

        /// <summary>
        /// Removes layers from a layer mask.
        /// </summary>
        /// <param name="layerNames">Layer names to remove.</param>
        /// <returns>Layer mask with removed layers.</returns>
        public static LayerMask RemoveLayers(this LayerMask layerMask, params string[] layerNames)
        {
            LayerMask invertedMask = ~layerMask;
            return ~(invertedMask | NamesToMask(layerNames));
        }

        /// <summary>
        /// Removes layers from a layer mask.
        /// </summary>
        /// <param name="layerIndexes">Layer indexes to remove.</param>
        /// <returns>Layer mask with removed layers.</returns>
        public static LayerMask RemoveLayers(this LayerMask layerMask, params int[] layerIndexes)
        {
            LayerMask invertedMask = ~layerMask;
            return ~(invertedMask | IndexesToMask(layerIndexes));
        }

        /// <summary>
        /// Inverses the mask.
        /// </summary>
        /// <returns>Inversed mask.</returns>
        public static LayerMask Inverse(this LayerMask layerMask)
        {
            return ~layerMask;
        }

        #endregion // MASK MANIPULATION

        #region VALUES TO MASK

        /// <summary>
        /// Creates a LayerMask from layer names.
        /// </summary>
        /// <param name="layerNames">Layer names to create mask from.</param>
        /// <returns>Mask created from layer names.</returns>
        public static LayerMask NamesToMask(params string[] layerNames)
        {
            LayerMask mask = 0;
            for (int i = 0; i < layerNames.Length; ++i)
                mask |= 1 << LayerMask.NameToLayer(layerNames[i]);

            return mask;
        }

        /// <summary>
        /// Creates a LayerMask from layer indexes.
        /// </summary>
        /// <param name="layerIndexes">Layer indexes to create mask from.</param>
        /// <returns>Mask created from layer indexes.</returns>
        public static LayerMask IndexesToMask(params int[] layerIndexes)
        {
            LayerMask mask = 0;
            for (int i = 0; i < layerIndexes.Length; ++i)
                mask |= 1 << layerIndexes[i];

            return mask;
        }

        #endregion // VALUES TO MASK
    }
}