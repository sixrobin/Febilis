namespace Templar
{
    using System.Collections.Generic;
    using RSLib.Extensions;
    using UnityEngine;
    using System.Linq;

    public class Test : MonoBehaviour
    {
        public RSLib.Framework.Events.GameEvent GameEvent;

        private void Start()
        {
            GameEvent.Raise();
        }

        public void PrintCoucou()
        {
            Debug.LogError("Coucou");
        }
    }
}