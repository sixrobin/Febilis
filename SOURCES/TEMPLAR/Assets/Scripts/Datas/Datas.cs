namespace Templar.Datas
{
    public abstract class Datas
    {
        public Datas(System.Xml.Linq.XContainer container)
        {
            if (container != null)
                Deserialize(container);
        }

        public abstract void Deserialize(System.Xml.Linq.XContainer container);
    }
}