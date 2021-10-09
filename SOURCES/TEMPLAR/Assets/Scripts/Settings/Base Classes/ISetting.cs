namespace Templar.Settings
{
    using System.Xml.Linq;

    public interface ISetting
    {
        void Init();
        void Load(XElement element);
        XElement Save();
    }
}