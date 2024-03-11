namespace RSLib.Extensions
{
    using System.Xml.Linq;

    public static class XDocumentExtensions
    {
        #region GENERAL

        /// <summary>
        /// Checks if an XAttribute or its value are null.
        /// </summary>
        public static bool IsNullOrEmpty(this XAttribute attribute)
        {
            return attribute == null || string.IsNullOrEmpty(attribute.Value);
        }

        /// <summary>
        /// Checks if an XElement or its value are null.
        /// </summary>
        public static bool IsNullOrEmpty(this XElement element)
        {
            return element == null || string.IsNullOrEmpty(element.Value);
        }

        #endregion // GENERAL

        #region PARSING

        /// <summary>
        /// Parses a XElement value to a float value.
        /// </summary>
        /// <returns>Element value if parsing succeeded, else throws an exception.</returns>
        public static float ValueToFloat(this XElement element)
        {
            UnityEngine.Assertions.Assert.IsFalse(element.IsNullOrEmpty(), $"XElement is null or empty.");

            if (float.TryParse(element.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value))
                return value;

            throw new System.Exception($"Could not parse XElement {element.Name.LocalName} Value {element.Value} to a valid float value.");
        }

        /// <summary>
        /// Parses a XAttribute value to a float value.
        /// </summary>
        /// <returns>Attribute value if parsing succeeded, else throws an exception.</returns>
        public static float ValueToFloat(this XAttribute attribute)
        {
            UnityEngine.Assertions.Assert.IsFalse(attribute.IsNullOrEmpty(), $"XAttribute is null or empty.");

            if (float.TryParse(attribute.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value))
                return value;

            throw new System.Exception($"Could not parse XAttribute {attribute.Name.LocalName} Value {attribute.Value} to a valid float value.");
        }

        /// <summary>
        /// Parses a XElement value to an integer value.
        /// </summary>
        /// <returns>Element value if parsing succeeded, else throws an exception.</returns>
        public static int ValueToInt(this XElement element)
        {
            UnityEngine.Assertions.Assert.IsFalse(element.IsNullOrEmpty(), $"XElement is null or empty.");

            if (int.TryParse(element.Value, out int value))
                return value;

            throw new System.Exception($"Could not parse XElement {element.Name.LocalName} Value {element.Value} to a valid integer value.");
        }

        /// <summary>
        /// Parses a XAttribute value to an integer value.
        /// </summary>
        /// <returns>Attribute value if parsing succeeded, else throws an exception.</returns>
        public static int ValueToInt(this XAttribute attribute)
        {
            UnityEngine.Assertions.Assert.IsFalse(attribute.IsNullOrEmpty(), $"XAttribute is null or empty.");

            if (int.TryParse(attribute.Value, out int value))
                return value;

            throw new System.Exception($"Could not parse XAttribute {attribute.Name.LocalName} Value {attribute.Value} to a valid integer value.");
        }

        /// <summary>
        /// Parses a XElement value to a long value.
        /// </summary>
        /// <returns>Element value if parsing succeeded, else throws an exception.</returns>
        public static long ValueToLong(this XElement element)
        {
            UnityEngine.Assertions.Assert.IsFalse(element.IsNullOrEmpty(), $"XElement is null or empty.");

            if (long.TryParse(element.Value, out long value))
                return value;

            throw new System.Exception($"Could not parse XElement {element.Name.LocalName} Value {element.Value} to a valid long value.");
        }

        /// <summary>
        /// Parses a XAttribute value to a long value.
        /// </summary>
        /// <returns>Attribute value if parsing succeeded, else throws an exception.</returns>
        public static long ValueToLong(this XAttribute attribute)
        {
            UnityEngine.Assertions.Assert.IsFalse(attribute.IsNullOrEmpty(), $"XAttribute is null or empty.");

            if (long.TryParse(attribute.Value, out long value))
                return value;

            throw new System.Exception($"Could not parse XAttribute {attribute.Name.LocalName} Value {attribute.Value} to a valid long value.");
        }

        /// <summary>
        /// Parses a XElement value to a boolean value.
        /// </summary>
        /// <returns>Element value if parsing succeeded, else throws an exception.</returns>
        public static bool ValueToBool(this XElement element)
        {
            UnityEngine.Assertions.Assert.IsFalse(element.IsNullOrEmpty(), $"XElement is null or empty.");

            if (bool.TryParse(element.Value, out bool value))
                return value;

            throw new System.Exception($"Could not parse XElement {element.Name.LocalName} Value {element.Value} to a valid boolean value.");
        }

        /// <summary>
        /// Parses a XAttribute value to a boolean value.
        /// </summary>
        /// <returns>Attribute value if parsing succeeded, else throws an exception.</returns>
        public static bool ValueToBool(this XAttribute attribute)
        {
            UnityEngine.Assertions.Assert.IsFalse(attribute.IsNullOrEmpty(), $"XAttribute is null or empty.");

            if (bool.TryParse(attribute.Value, out bool value))
                return value;

            throw new System.Exception($"Could not parse XAttribute {attribute.Name.LocalName} Value {attribute.Value} to a valid boolean value.");
        }

        /// <summary>
        /// Parses a XElement value to an enum value.
        /// </summary>
        /// <returns>Element value if parsing succeeded, else throws an exception.</returns>
        public static T ValueToEnum<T>(this XElement element) where T : System.Enum
        {
            UnityEngine.Assertions.Assert.IsFalse(element.IsNullOrEmpty(), $"XElement is null or empty.");

            if (System.Enum.IsDefined(typeof(T), element.Value))
                return (T)System.Enum.Parse(typeof(T), element.Value);

            throw new System.Exception($"Could not parse XElement {element.Name.LocalName} Value {element.Value} to a valid enum value.");

        }

        /// <summary>
        /// Parses a XAttribute value to an enum value.
        /// </summary>
        /// <returns>Element value if parsing succeeded, else throws an exception.</returns>
        public static T ValueToEnum<T>(this XAttribute attribute) where T : System.Enum
        {
            UnityEngine.Assertions.Assert.IsFalse(attribute.IsNullOrEmpty(), $"XAttribute is null or empty.");

            if (System.Enum.IsDefined(typeof(T), attribute.Value))
                return (T)System.Enum.Parse(typeof(T), attribute.Value);

            throw new System.Exception($"Could not parse XAttribute {attribute.Name.LocalName} Value {attribute.Value} to a valid enum value.");
        }

        /// <summary>
        /// Parses a XElement Min and Max attributes to a float Tuple.
        /// </summary>
        /// <returns>Attributes values if parsing succeeded, else, each failed value will be 0.</returns>
        public static (float min, float max) MinMaxAttributesToFloats(this XElement element)
        {
            UnityEngine.Assertions.Assert.IsTrue(element != null, $"XElement is null.");

            XAttribute minAttribute = element.Attribute("Min");
            UnityEngine.Assertions.Assert.IsFalse(minAttribute.IsNullOrEmpty(), $"Min attribute is missing or empty.");

            XAttribute maxAttribute = element.Attribute("Max");
            UnityEngine.Assertions.Assert.IsFalse(maxAttribute.IsNullOrEmpty(), $"Max attribute is missing or empty.");

            float min = minAttribute.ValueToFloat();
            float max = maxAttribute.ValueToFloat();
            return (min, max);
        }

        #endregion // PARSING

        #region TRY GET

        /// <summary>
        /// Tries to get a XElement by its name, and returns true or false depending on the XElement existence.
        /// </summary>
        /// <param name="container">Xml container to get the element from.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="element">Element returned if it exists.</param>
        /// <returns>True if the element exists.</returns>
        public static bool TryGetElement(this XContainer container, string elementName, out XElement element)
        {
            element = container.Element(elementName);
            return element != null;
        }

        /// <summary>
        /// Tries to get a XAttribute by its name, and returns true or false depending on the XAttribute existence.
        /// </summary>
        /// <param name="container">Xml container to get the attribute from.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="attribute">Attribute returned if it exists.</param>
        /// <returns>True if the attribute exists.</returns>
        public static bool TryGetAttribute(this XElement container, string attributeName, out XAttribute attribute)
        {
            attribute = container.Attribute(attributeName);
            return attribute != null;
        }

        #endregion // TRY GET
    }
}