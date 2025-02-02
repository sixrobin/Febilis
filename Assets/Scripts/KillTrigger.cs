﻿namespace Templar
{
    using UnityEngine;

    public class KillTrigger : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider = null;

        public static System.Collections.Generic.Dictionary<Collider2D, KillTrigger> SharedKillTriggers { get; }
            = new System.Collections.Generic.Dictionary<Collider2D, KillTrigger>();

        public Collider2D Collider => _collider;

        public static void ResetSharedTriggers()
        {
            // Used to cache all KillTriggers in the scene, and avoid to do lots of GetComponent calls later.

            SharedKillTriggers.Clear();

            KillTrigger[] killTriggers = FindObjectsOfType<KillTrigger>();
            for (int i = killTriggers.Length - 1; i >= 0; --i)
                SharedKillTriggers.Add(killTriggers[i].Collider, killTriggers[i]);
        }

        private void Awake()
        {
            if (_collider == null)
            {
                Debug.LogWarning($"Collider is not referenced on {transform.name}, trying GetComponent.", gameObject);
                _collider = GetComponent<Collider2D>();
            }
        }
    }
}