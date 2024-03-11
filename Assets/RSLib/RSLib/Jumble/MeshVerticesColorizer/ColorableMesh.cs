namespace RSLib.Jumble.MeshVerticesColorizer
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class ColorableMesh : MonoBehaviour
	{
		[Header ("VERTICES COLORS")]
		[SerializeField] Color _baseVerticesColor = Color.red;

		[Header ("BLEND")]
		[SerializeField] float _blendDuration = 1f;

		private Mesh _mesh;
		private int _meshVertices;
		private bool _coloring;

		private Color[] _meshColors;
		private Color[] _palette;
		private List<int> _verticesToColor = new List<int>();
		private float[] _colorationPercentages;

		/// <summary>
		/// Initializes mesh colors and coloring arrays.
		/// </summary>
		private void Initialize()
		{
			_meshVertices = _mesh.vertices.Length;

			_meshColors = new Color[_meshVertices];
			for (int i = 0; i < _meshVertices; ++i)
				_meshColors[i] = _baseVerticesColor;

			_colorationPercentages = new float[_meshVertices];
			_palette = new Color[_meshVertices];
			_mesh.colors = _meshColors;
		}

		/// <summary>
		/// Colors any vertex that needs to be colored.
		/// Coroutines runs while there's at least one vertex that is still getting colored.
		/// </summary>
		private IEnumerator ColorVerticesCoroutine()
		{
			_coloring = true;

			while (_verticesToColor.Count > 0)
			{
                List<int> toRemove = new List<int>();

				foreach (int vertexIndex in _verticesToColor)
				{
					_colorationPercentages[vertexIndex] += Time.deltaTime / _blendDuration;
					_meshColors[vertexIndex] = Color.Lerp(_baseVerticesColor, _palette[vertexIndex], _colorationPercentages[vertexIndex]);

					if (_colorationPercentages[vertexIndex] > 1)
						toRemove.Add(vertexIndex);
				}

				foreach (int index in toRemove)
					_verticesToColor.Remove(index);

				_mesh.colors = _meshColors;
				yield return null;
			}

			_coloring = false;
		}

		/// <summary>
		/// Gets vertices to color and stores their index so that the ColorVertices coroutine can color them.
		/// </summary>
		/// <param name="worldPos">Reference painting point.</param>
		/// <param name="radius">Radius in which to paint vertices.</param>
		/// <param name="color">Target vertex color.</param>
		public void ColorAtWorldPosition(Vector3 worldPos, float radius, Color color)
		{
			for (int i = 0; i < _meshVertices; ++i)
			{
				Vector3 vertexToWorld = transform.localToWorldMatrix.MultiplyPoint3x4(_mesh.vertices[i]);

				if ((vertexToWorld - worldPos).sqrMagnitude < radius * radius)
                {
					if (!_verticesToColor.Contains(i))
					{
						_verticesToColor.Add(i);
						_palette[i] = color;
					}
                }
			}

			if (!_coloring && _verticesToColor.Count > 0)
				StartCoroutine(ColorVerticesCoroutine());
		}

		private void Awake()
		{
			_mesh = GetComponent<MeshFilter>().mesh;
			Initialize();
		}

		private void OnValidate()
		{
			_blendDuration = Mathf.Clamp(_blendDuration, 0.01f, float.MaxValue);
		}
	}
}