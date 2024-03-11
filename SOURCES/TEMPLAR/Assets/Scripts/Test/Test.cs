namespace Templar
{
    using System.Collections.Generic;
    using RSLib.Yield;
    using UnityEngine;
    using System.Linq;
    using RSLib.Extensions;

    public class Test : MonoBehaviour
    {
        private void Start()
        {
            transform.name = "Oui (Clone)";
            gameObject.RemoveCloneFromName();
        }
    }
}