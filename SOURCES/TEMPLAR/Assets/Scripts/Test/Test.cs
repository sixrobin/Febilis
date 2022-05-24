namespace Templar
{
    using System.Collections.Generic;
    using RSLib.Extensions;
    using UnityEngine;
    using System.Linq;

    public class Test : MonoBehaviour
    {
        public RSLib.ImageEffects.ColorFlashScriptable data;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RSLib.ImageEffects.ColorFlash.Flash(data,
                                                    inCallback: () => Debug.Log("A"),
                                                    outCallback: () => Debug.Log("B"));
            }
        }
    }
}