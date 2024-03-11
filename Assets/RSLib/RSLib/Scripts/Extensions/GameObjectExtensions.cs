namespace RSLib.Extensions
{
    using UnityEngine;

    public static class GameObjectExtensions
    {
        #region COMPONENT

        /// <summary>
        /// Gets the specified component and adds it if it was not already on the gameObject.
        /// </summary>
        /// <returns>Existing or added component.</returns>
        public static T AddComponentIfMissing<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();

            if (component == null)
                component = go.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// Gets the specified component on the gameObject or one of its parents.
        /// </summary>
        public static T FindInParents<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();

            if (component == null)
            {
                Transform parent = go.transform.parent;

                while (parent != null && component == null)
                {
                    component = parent.gameObject.GetComponent<T>();
                    parent = parent.parent;
                }
            }

            return component;
        }

        #endregion // COMPONENT

        #region GENERAL

        /// <summary>
        /// Removes the first occurrence of the "(Clone)" string added to any instantiated prefab.
        /// </summary>
        public static void RemoveCloneFromName(this GameObject go)
        {
            const string toRemove = "(Clone)";
            int index = go.name.IndexOf(toRemove);
            go.name = index < 0 ? go.name : go.name.Remove(index, toRemove.Length);
        }

        /// <summary>
        /// Transfers all children of a gameObject to another parent.
        /// </summary>
        /// <param name="go">GameObject of which to transfer children.</param>
        /// <param name="newParent">New parent transform.</param>
        public static void TransferChildren(this GameObject go, Transform newParent)
        {
            Transform[] children = new Transform[go.transform.childCount];
            for (int i = children.Length - 1; i >= 0; --i)
                children[i] = go.transform.GetChild(i);

            for (int i = children.Length - 1; i >= 0; --i)
            {
                children[i].SetParent(newParent);
                children[i].SetAsFirstSibling();
            }
        }

        #endregion // GENERAL

        #region LAYER

        /// <summary>
        /// Sets the layer of the gameObject and all its children and sub children.
        /// </summary>
        /// <param name="go">GameObject to set layer and its children of.</param>
        /// <param name="layer">Layer index.</param>
        public static void SetChildrenLayers(this GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                child.gameObject.SetChildrenLayers(layer);
        }

        /// <summary>
        /// Sets the layer of the gameObject and all its children and sub children.
        /// </summary>
        /// <param name="go">GameObject to set layer and its children of.</param>
        /// <param name="layerName">Layer name.</param>
        public static void SetChildrenLayers(this GameObject go, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);

            go.layer = layer;
            foreach (Transform child in go.transform)
                child.gameObject.SetChildrenLayers(layer);
        }

        #endregion // LAYER
    }
}