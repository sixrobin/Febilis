namespace Templar
{
    using System.Collections.Generic;
    using RSLib.Extensions;
    using UnityEngine;
    using System.Linq;

    public class Test : MonoBehaviour
    {
        public RSLib.Data.Float DataFloat;
        public float f;
        
        private void Start()
        {
            DataFloat.ValueChanged += OnValueChanged;
            f += DataFloat;
        }

        private void OnValueChanged(RSLib.Data.Float.ValueChangedEventArgs args)
        {
            Debug.LogError(args.New);
        }

        private void Update()
        {
        }
    }
}