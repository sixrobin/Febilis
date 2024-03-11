namespace RSLib.Data.Editor
{
    [UnityEditor.CustomPropertyDrawer(typeof(FloatField))]
    public class FloatFieldPropertyDrawer : DataFieldPropertyDrawer
    {
        protected override string DataFieldName => "_dataFloat";
        protected override string ValueFieldName => "_valueFloat";
    }
}