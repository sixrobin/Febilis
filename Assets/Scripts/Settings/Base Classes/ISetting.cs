namespace Templar.Settings
{
    using System.Xml.Linq;

    public interface ISetting
    {
        void Init();
        bool CanBeDisplayedToUser();

        void Load(XElement element);
        XElement Save();
    }
}