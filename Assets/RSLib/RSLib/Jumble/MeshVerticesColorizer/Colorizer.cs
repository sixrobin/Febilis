namespace RSLib.Jumble.MeshVerticesColorizer
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Class used to raycast objects in scene and look for ColorableMesh instances.
	/// Any ColorableMesh found will have its mesh vertices painted using the Colorizer settings.
	/// </summary>
	public class Colorizer : MonoBehaviour
	{
		[Header ("DETECTION SETTINGS (Raycasts to forward)")]
		[SerializeField] private float _travelDistToUpdate = 1f;
		[SerializeField] private float _detectionDist = 1000f;
		[SerializeField] private float _colorRadius = 3f;
		[SerializeField] private LayerMask _mask = 0;

		[Header ("COLOR SETTINGS")]
		[SerializeField] private Color _paintColor = Color.green;

		[Header ("DEBUG")]
		[SerializeField] private bool _showDebug = true;

		private Vector3 _lastRecordedPos;
		private Dictionary<Transform, ColorableMesh> _alreadyKnownColorables = new Dictionary<Transform, ColorableMesh>();

		/// <summary>
		/// Raycasts to find a ColorableMesh. If one is found, checks if it is already known or not, then colors it.
		/// </summary>
		private void LookForColorableMesh()
		{
			if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _detectionDist, _mask))
				return;

            Transform hitTransform = hit.transform;

			if (_alreadyKnownColorables.ContainsKey(hitTransform))
			{
				if (_showDebug)
					Debug.Log($"{nameof(Colorizer)}: Detected an already known ColorableMesh {hitTransform.name}. Distance : {hit.distance}.", gameObject);
				
				_alreadyKnownColorables[hitTransform].ColorAtWorldPosition(hit.point, _colorRadius, _paintColor);
				return;
			}

			if (hitTransform.TryGetComponent(out ColorableMesh colorable))
			{
				if (_showDebug)
					Debug.Log($"{nameof(Colorizer)}: Detected a new ColorableMesh {hitTransform.name}. Distance : {hit.distance}.", gameObject);

				colorable.ColorAtWorldPosition(hit.point, _colorRadius, _paintColor);
				_alreadyKnownColorables.Add(hit.transform, colorable);
			}
		}

		private void Awake()
		{
			_lastRecordedPos = transform.position;
		}

		private void Update()
		{
			if (_showDebug)
				Debug.DrawLine (transform.position, transform.position + transform.forward * _detectionDist, Color.red);

			if ((transform.position - _lastRecordedPos).sqrMagnitude > _travelDistToUpdate * _travelDistToUpdate)
			{
				if (_showDebug)
					Debug.Log($"{nameof(Colorizer)}: Looking for a mesh to color.", gameObject);

				LookForColorableMesh();
				_lastRecordedPos = transform.position;
			}
		}

		private void OnValidate()
		{
			_travelDistToUpdate = Mathf.Clamp(_travelDistToUpdate, 0.01f, float.MaxValue);
			_colorRadius = Mathf.Clamp(_colorRadius, 0, float.MaxValue);
			_detectionDist = Mathf.Clamp(_detectionDist, 0, float.MaxValue);
		}
	}
}