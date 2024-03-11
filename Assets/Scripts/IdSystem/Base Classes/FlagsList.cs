namespace Templar.Flags
{
    using System.Xml.Linq;

    public class FlagsList
    {
        private System.Collections.Generic.List<string> _ids = new System.Collections.Generic.List<string>();

        private System.Type _identifierType;
        private bool _silentDuplicatesMerge;

        public FlagsList(System.Type identifierType, bool silentDuplicatesMerge = false)
        {
            _identifierType = identifierType;
            _silentDuplicatesMerge = silentDuplicatesMerge;
        }

        public bool Check(IIdentifiable identifiable)
        {
            return Check(identifiable.Identifier.Id);
        }

        public bool Check(string id)
        {
            return _ids.Contains(id);
        }

        public void Register(IIdentifiable identifiable)
        {
            Register(identifiable.Identifier.Id);
        }

        private void Register(string id)
        {
            bool idChecked = Check(id);

            UnityEngine.Assertions.Assert.IsTrue(
                _silentDuplicatesMerge || !idChecked,
                $"Adding already registered Id {id} to the {_identifierType.Name} list.");

            if (!idChecked)
                _ids.Add(id);
        }

        public void Load(XElement idsElement)
        {
            UnityEngine.Assertions.Assert.IsNotNull(idsElement, $"Loading a null XElement into {_identifierType.Name} {GetType().Name}.");

            foreach (XElement idElement in idsElement.Elements("Id"))
                Register(idElement.Value);
        }

        public XElement Save()
        {
            if (_ids.Count == 0)
                return null;

            _ids.Sort();

            XElement idsElement = new XElement(_identifierType.Name);
            _ids.ForEach(o => idsElement.Add(new XElement("Id", o)));
            return idsElement;
        }
    }
}
