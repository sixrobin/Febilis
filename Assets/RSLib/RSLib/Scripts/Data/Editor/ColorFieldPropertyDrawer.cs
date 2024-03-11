namespace RSLib.Data.Editor
{
    [UnityEditor.CustomPropertyDrawer(typeof(ColorField))]
    public class ColorFieldPropertyDrawer : DataFieldPropertyDrawer
    {
        protected override string DataFieldName => "_dataColor";
        protected override string ValueFieldName => "_valueColor";
    }
}