namespace Templar.Datas
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class ShakeTraumaDatas
    {
        public ShakeTraumaDatas()
        {
        }

        public ShakeTraumaDatas(XContainer container)
        {
            Deserialize(container);
        }

        public enum ShakeAddType
        {
            ADDITIVE,
            OVERRIDE
        }

        public static ShakeTraumaDatas Default => new ShakeTraumaDatas()
        {
            X = 0.25f,
            Y = 0.25f,
            AddType = ShakeAddType.ADDITIVE,
            ShakeId = Camera.CameraShake.ID_MEDIUM
        };

        public string ShakeId { get; private set; }

        public float X { get; private set; }
        public float Y { get; private set; }
        public ShakeAddType AddType { get; private set; }

        public bool CanShakeWhenOffscreen { get; private set; }

        public void Deserialize(XContainer container)
        {
            XElement traumaElement = container as XElement;

            XAttribute shakeIdElement = traumaElement.Attribute("ShakeId");
            ShakeId = shakeIdElement?.Value ?? Camera.CameraShake.ID_MEDIUM;

            XAttribute xAttribute = traumaElement.Attribute("X");
            UnityEngine.Assertions.Assert.IsNotNull(xAttribute, "ShakeTraumaDatas must have a X attribute.");
            X = xAttribute.ValueToFloat();

            XAttribute yAttribute = traumaElement.Attribute("Y");
            UnityEngine.Assertions.Assert.IsNotNull(yAttribute, "ShakeTraumaDatas must have an Y attribute.");
            Y = yAttribute.ValueToFloat();

            UnityEngine.Assertions.Assert.IsTrue(X >= 0 && X <= 1, "ShakeTraumaDatas X value must be contained between 0 and 1.");
            UnityEngine.Assertions.Assert.IsTrue(Y >= 0 && Y <= 1, "ShakeTraumaDatas Y value must be contained between 0 and 1.");

            XAttribute addTypeAttribute = traumaElement.Attribute("AddType");
            AddType = addTypeAttribute?.ValueToEnum<ShakeAddType>() ?? ShakeAddType.ADDITIVE;

            XAttribute canShakeWhenOffscreen = traumaElement.Attribute("CanShakeWhenOffscreen");
            CanShakeWhenOffscreen = canShakeWhenOffscreen?.ValueToBool() ?? false;
        }
    }
}