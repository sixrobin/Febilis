namespace RSLib.Framework.GameSettings
{
    public abstract class GameSetting
    {
        public GameSetting()
        {
            Init();
        }

        public GameSetting(System.Xml.Linq.XElement saveElement)
        {
            Load(saveElement);
        }
        
        public abstract string SerializationName { get; }

        public virtual bool UserAssignable => true;
        
        protected abstract void Init();
        
        public abstract void Load(System.Xml.Linq.XElement saveData);
        public abstract System.Xml.Linq.XElement Save();
    }
}