namespace Templar
{
    using System.Collections.Generic;
    using RSLib.Yield;
    using UnityEngine;
    using System.Linq;
    using RSLib.Extensions;

    public class Test : MonoBehaviour
    {
        [Range(0,1)]
        public float t = 0.5f;
        
        private void Update()
        {
            transform.SetPositionX(Vector2.right.LerpUnclamped(t, RSLib.Maths.Curve.OutBack));
        }
    }
}