namespace Templar.Database
{
    using System.Xml.Linq;
    using UnityEngine;

    public class DestroyablesDatabase : RSLib.Framework.ConsoleProSingleton<DestroyablesDatabase>, IDatabase
    {
        [SerializeField] private TextAsset _destroyablesDatas = null;

        public static System.Collections.Generic.Dictionary<string, Datas.DestroyableDatas> DestroyablesDatas { get; private set; }

        void IDatabase.Load()
        {
            DeserializeDestroyablesDatas();
        }

        private void DeserializeDestroyablesDatas()
        {
            DestroyablesDatas = new System.Collections.Generic.Dictionary<string, Datas.DestroyableDatas>();

            XDocument destroyablesDatasDoc = XDocument.Parse(_destroyablesDatas.text, LoadOptions.SetBaseUri);
            XElement destroyablesDatasElement = destroyablesDatasDoc.Element("DestroyablesDatas");

            foreach (XElement destroyableDatasElement in destroyablesDatasElement.Elements("DestroyableDatas"))
            {
                Datas.DestroyableDatas destroyableDatas = new Datas.DestroyableDatas(destroyableDatasElement);
                DestroyablesDatas.Add(destroyableDatas.Id, destroyableDatas);
            }

            Log($"Deserialized {DestroyablesDatas.Count} destroyable objects datas.");
        }
    }
}