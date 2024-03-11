namespace RSLib.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data LayerMask", menuName = "RSLib/Data/LayerMask")]
    public class LayerMask : ScriptableObject
    {
        [SerializeField] private UnityEngine.LayerMask _mask = 0;

        public UnityEngine.LayerMask Mask => _mask;
        
#region CONVERSION OPERATORS
        
        public static implicit operator UnityEngine.LayerMask(LayerMask dataLayerMask)
        {
            return dataLayerMask.Mask;
        }

        public static implicit operator int(LayerMask dataLayerMask)
        {
            return (int)dataLayerMask.Mask;
        }
        
#endregion // CONVERSION OPERATORS
        
        public override string ToString()
        {
            return Mask.ToString();
        }
    }
}