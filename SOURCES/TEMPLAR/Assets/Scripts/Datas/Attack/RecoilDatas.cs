namespace Templar.Datas.Attack
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class RecoilDatas : Datas
    {
        public RecoilDatas() : base(null)
        {
        }

        public RecoilDatas(XContainer container) : base(container)
        {
        }

        public RecoilDatas(float force, float dur, float airborneMult = 1f, bool checkEdge = false) : base(null)
        {
            Force = force;
            Dur = dur;
            AirborneMult = airborneMult;
            CheckEdge = checkEdge;
        }

        public static RecoilDatas Default => new RecoilDatas()
        {
            Force = 10,
            Dur = 0.25f,
            AirborneMult = 1f,
            CheckEdge = false
        };

        public static RecoilDatas NullRecoil => new RecoilDatas()
        {
            Force = 0f,
            Dur = 0f,
            AirborneMult = 1f,
            CheckEdge = false
        };

        public float Force { get; private set; }
        public float Dur { get; private set; }
        public float AirborneMult { get; private set; }
        public bool CheckEdge { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement recoilElement = container as XElement;

            XElement forceElement = recoilElement.Element("Force");
            UnityEngine.Assertions.Assert.IsFalse(forceElement.IsNullOrEmpty(), "Recoil datas needs a Force element.");
            Force = forceElement.ValueToFloat();

            XElement durElement = recoilElement.Element("Dur");
            UnityEngine.Assertions.Assert.IsFalse(durElement.IsNullOrEmpty(), "Recoil datas needs a Dur element.");
            Dur = durElement.ValueToFloat();

            XElement airborneMultElement = recoilElement.Element("AirborneMult");
            AirborneMult = airborneMultElement?.ValueToFloat() ?? 1f;

            CheckEdge = recoilElement.Element("CheckEdge") != null;
        }
    }
}