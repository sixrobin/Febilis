namespace RSLib.Data.Editor
{
    [UnityEditor.CustomPropertyDrawer(typeof(IntField))]
    public class IntFieldPropertyDrawer : DataFieldPropertyDrawer
    {
        protected override string DataFieldName => "_dataInt";
        protected override string ValueFieldName => "_valueInt";
    }
}