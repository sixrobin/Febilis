namespace RSLib.Framework.Events
{
    [System.Serializable]
    public class BoolEvent : UnityEngine.Events.UnityEvent<bool> { }

    [System.Serializable]
    public class IntEvent : UnityEngine.Events.UnityEvent<int> { }

    [System.Serializable]
    public class FloatEvent : UnityEngine.Events.UnityEvent<float> { }

    [System.Serializable]
    public class StringEvent : UnityEngine.Events.UnityEvent<string> { }

    [System.Serializable]
    public class ColliderEvent : UnityEngine.Events.UnityEvent<UnityEngine.Collider> { }

    [System.Serializable]
    public class Collider2DEvent : UnityEngine.Events.UnityEvent<UnityEngine.Collider2D> { }

    [System.Serializable]
    public class CollisionEvent : UnityEngine.Events.UnityEvent<UnityEngine.Collision> { }

    [System.Serializable]
    public class Collision2DEvent : UnityEngine.Events.UnityEvent<UnityEngine.Collision2D> { }

    [System.Serializable]
    public class Vector2Event : UnityEngine.Events.UnityEvent<UnityEngine.Vector2> { }

    [System.Serializable]
    public class Vector2IntEvent : UnityEngine.Events.UnityEvent<UnityEngine.Vector2Int> { }

    [System.Serializable]
    public class Vector3Event : UnityEngine.Events.UnityEvent<UnityEngine.Vector3> { }

    [System.Serializable]
    public class Vector3IntEvent : UnityEngine.Events.UnityEvent<UnityEngine.Vector3Int> { }

    [System.Serializable]
    public class QuaternionEvent : UnityEngine.Events.UnityEvent<UnityEngine.Quaternion> { }

    [System.Serializable]
    public class ColorEvent : UnityEngine.Events.UnityEvent<UnityEngine.Color> { }
}