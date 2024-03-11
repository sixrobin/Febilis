namespace RSLib.Editor
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditorInternal;

	/// <summary>
	/// Allows to draw a layer mask in a custom inspector.
	/// </summary>
	public class LayerMaskFieldEditor
	{
		private static List<int> s_layerNumbers = new List<int>();

		public static LayerMask LayerMaskField(string label, LayerMask layerMask)
		{
			string[] layers = InternalEditorUtility.layers;
			s_layerNumbers.Clear();

			for (int i = 0; i < layers.Length; ++i)
				s_layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

			int maskWithoutEmpty = 0;
			for (int i = 0; i < s_layerNumbers.Count; ++i)
				if (((1 << s_layerNumbers[i]) & layerMask.value) > 0)
					maskWithoutEmpty |= 1 << i;

			maskWithoutEmpty = UnityEditor.EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

			int mask = 0;
			for (int i = 0; i < s_layerNumbers.Count; ++i)
				if ((maskWithoutEmpty & (1 << i)) > 0)
					mask |= 1 << s_layerNumbers[i];

			layerMask.value = mask;
			return layerMask;
		}
	}
}