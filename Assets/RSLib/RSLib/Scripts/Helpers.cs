namespace RSLib
{
    using System.Linq;

    public static class Helpers
    {
        private static System.Random s_rnd = new System.Random();

        #region BOOLEAN

        /// <summary>
        /// Computes a random boolean.
        /// </summary>
        /// <returns>Computed boolean.</returns>
        public static bool CoinFlip()
        {
            return s_rnd.Next(2) == 0;
        }

        /// <summary>
        /// Computes a random boolean using a weight.
        /// </summary>
        /// <param name="percentage01">Chances of returning true, between 0 and 1.</param>
        /// <returns>Computed boolean.</returns>
        public static bool CoinFlip(float percentage01)
        {
            return s_rnd.Next(101) < percentage01 * 100f;
        }

        #endregion // BOOLEAN

        #region ENUM

        /// <summary>
        /// Computes all values of a given Enum type into an array.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <returns>Array with all Enum values.</returns>
        public static T[] GetEnumValues<T>() where T : System.Enum
        {
            return System.Enum.GetValues(typeof(T)) as T[];
        }

        /// <summary>
        /// Computes all values of a given Enum type into an array of their integer values.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <returns>Array with all integer values.</returns>
        public static int[] GetEnumIntValues<T>() where T : System.Enum
        {
            return System.Enum.GetValues(typeof(T)) as int[];
        }

        #endregion // ENUM

        #region FIND

        /// <summary>
        /// Same method as UnityEngine.Object.FindObjectsOfType that looks for every MonoBehaviour and then filters by
        /// the specified type, allowing it to also find instances of an interface.
        /// This method is slower than UnityEngine.Object.FindObjectsOfType, so use it very carefully or for editor purpose.
        /// </summary>
        /// <typeparam name="T">Type to look for.</typeparam>
        /// <returns>IEnumerable containing the instances of the searched type.</returns>
        public static System.Collections.Generic.IEnumerable<T> FindInstancesOfType<T>()
        {
            return UnityEngine.Object.FindObjectsOfType<UnityEngine.MonoBehaviour>().OfType<T>();
        }

        #endregion // FIND

        #region GUI

        /// <summary>
        /// Automatically adjust the ScrollView content position so that navigating through the slots with a controller
        /// works without having to move the Scrollbar manually.
        /// This can also handle mouse hovering if called in OnPointerEnter method.
        /// </summary>
        /// <param name="focusedRect">Item focused inside the ScrollView.</param>
        /// <param name="scrollViewViewport">ScrollView to adjust content viewport of.</param>
        /// <param name="scrollbar">Related Scrollbar.</param>
        /// <param name="stepValue">Step size to apply while item is not focused correctly.</param>
        /// <param name="margin">Margin to use to check if focus is correct.</param>
        /// <param name="snap">Scroll bar percentage below which it can snap to either 0 or 1.</param>
        public static void AdjustScrollViewToFocusedItem(UnityEngine.RectTransform focusedRect, UnityEngine.RectTransform scrollViewViewport, UnityEngine.UI.Scrollbar scrollbar, float stepValue, float margin, float? snap = null)
        {
            UnityEngine.Vector3[] sourceCorners = new UnityEngine.Vector3[4];
            UnityEngine.Vector3[] slotsViewportWorldCorners = new UnityEngine.Vector3[4];

            focusedRect.GetWorldCorners(sourceCorners);
            scrollViewViewport.GetWorldCorners(slotsViewportWorldCorners);

            while (sourceCorners[1].y > slotsViewportWorldCorners[1].y)
            {
                scrollbar.value += stepValue;
                focusedRect.GetWorldCorners(sourceCorners);
            }

            while (sourceCorners[0].y < slotsViewportWorldCorners[0].y)
            {
                scrollbar.value -= stepValue;
                focusedRect.GetWorldCorners(sourceCorners);
            }

            if (scrollbar.value - margin < 0f || (snap.HasValue && scrollbar.value < snap.Value))
                scrollbar.value = 0f;
            else if (scrollbar.value + margin > 1f || (snap.HasValue && scrollbar.value > 1f - snap.Value))
                scrollbar.value = 1f;
        }

        #endregion // GUI

        #region MISC

        /// <summary>
        /// Gets the local IP address to string.
        /// </summary>
        /// <returns>IP address to string.</returns>
        public static string GetLocalIPAddress()
        {
            System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (System.Net.IPAddress ip in host.AddressList)
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip.ToString();

            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// Copies a string value to clipboard, using GUIUtility.systemCopyBuffer.
        /// </summary>
        /// <param name="str">Value to copy to the clipboard.</param>
        public static void CopyToClipboard(string str)
        {
            UnityEngine.GUIUtility.systemCopyBuffer = str;
        }

        /// <summary>
        /// Quits application while dealing with current platform (Unity editor, windows, etc.).
        /// </summary>
        public static void QuitPlatformDependent()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
        
        /// <summary>
        /// Computes the average position between transforms.
        /// This method uses Linq and a foreach loop, using an array would be better if possible.
        /// </summary>
        /// <param name="transforms">Collection of transforms.</param>
        /// <returns>Computed position as a new Vector3.</returns>
        public static UnityEngine.Vector3 ComputeAveragePosition(System.Collections.Generic.IEnumerable<UnityEngine.Transform> transforms)
        {
            UnityEngine.Vector3 average = UnityEngine.Vector3.zero;

            foreach (UnityEngine.Transform t in transforms)
                average += t.position;

            average /= transforms.Count();
            return average;
        }

        /// <summary>
        /// Computes the average position between transforms.
        /// </summary>
        /// <param name="transforms">Array of transforms, or multiple transforms as multiple arguments.</param>
        /// <returns>Computed position as a new Vector3.</returns>
        public static UnityEngine.Vector3 ComputeAveragePosition(params UnityEngine.Transform[] transforms)
        {
            UnityEngine.Vector3 average = UnityEngine.Vector3.zero;

            for (int vectorIndex = transforms.Length - 1; vectorIndex >= 0; --vectorIndex)
                average += transforms[vectorIndex].position;

            average /= transforms.Length;
            return average;
        }

        /// <summary>
        /// Computes the average position between components' transforms.
        /// This method uses Linq and a foreach loop, using an array would be better if possible.
        /// </summary>
        /// <param name="components">Collection of components.</param>
        /// <returns>Computed position as a new Vector3.</returns>
        public static UnityEngine.Vector3 ComputeAveragePosition<T>(System.Collections.Generic.IEnumerable<T> components) where T : UnityEngine.Component
        {
            UnityEngine.Vector3 average = UnityEngine.Vector3.zero;

            foreach (T component in components)
                average += component.transform.position;

            average /= components.Count();
            return average;
        }

        /// <summary>
        /// Computes the average position between components.
        /// </summary>
        /// <param name="transforms">Array of transforms.</param>
        /// <returns>Computed position as a new Vector3.</returns>
        public static UnityEngine.Vector3 ComputeAveragePosition<T>(params T[] transforms) where T : UnityEngine.Component
        {
            UnityEngine.Vector3 average = UnityEngine.Vector3.zero;

            for (int vectorIndex = transforms.Length - 1; vectorIndex >= 0; --vectorIndex)
                average += transforms[vectorIndex].transform.position;

            average /= transforms.Length;
            return average;
        }

        /// <summary>
        /// Checks if an element equals at least one in a list of elements.
        /// </summary>
        /// <param name="source">Element to check.</param>
        /// <param name="list">Elements to compare.</param>
        /// <returns>True if one of the elements is the list equals the checked one.</returns>
        public static bool In<T>(this T source, params T[] list)
        {
            for (int i = list.Length - 1; i >= 0; --i)
                if (list[i].Equals(source))
                    return true;

            return false;
        }

        /// <summary>
        /// Scans a collection, looking for duplicate values. If any is found, the value and the number of occurences will be logged to the console.
        /// This method should only be used for editor purpose as it lacks optimization but only logs.
        /// </summary>
        /// <param name="collection">Collection to scan.</param>
        public static void ScanDuplicates<T>(this System.Collections.Generic.IEnumerable<T> collection)
        {
            System.Collections.Generic.Dictionary<T, int> duplicates = collection
                .GroupBy(o => o)
                .Where(o => o.Count() > 1)
                .ToDictionary(o => o.Key, o => o.Count());

            if (duplicates.Count == 0)
                UnityEngine.Debug.Log($"No duplicate has been found in the collection.");
            else
                foreach (System.Collections.Generic.KeyValuePair<T, int> duplicate in duplicates)
                    UnityEngine.Debug.Log($"Value {duplicate.Key} has been found {duplicate.Value} times in the collection.");
        }

        #endregion // MISC

        #region MODULO

        /// <summary>
        /// Custom modulo operating method to handle negative values.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="n">Second operand.</param>
        /// <returns>Modulo result.</returns>
        public static int Mod(int a, int n)
        {
            return (a % n + n) % n;
        }

        #endregion // MODULO
    }
}