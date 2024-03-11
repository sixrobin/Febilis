namespace RSLib.Jumble
{
	using UnityEngine;

	/// <summary>
	/// This class is used to set a humanoid avatar feet on ground.
	/// The animator layer needs to have "IK Pass" enable to trigger OnAnimatorIK callback.
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class IKFeetPlacement : MonoBehaviour
	{
		private const string LEFT_FOOT_ANIM_VAR_NAME = "LeftFootCurve";
		private const string RIGHT_FOOT_ANIM_VAR_NAME = "RightFootCurve";

		private Vector3 _leftFootPosition;
		private Vector3 _rightFootPosition;
		private Vector3 _leftFootIKPosition;
		private Vector3 _rightFootIKPosition;
		private Quaternion _leftFootIKRotation;
		private Quaternion _rightFootIKRotation;
		private float _lastPelvisPositionY;
		private float _lastLeftFootPositionY;
		private float _lastRightFootPositionY;

		public bool enableFeetIK = true;

		[Header("SETTINGS")]
		[SerializeField] private Animator _animator = null;
		[SerializeField, Range (0f, 2f)] private float _heightFromGroundRaycast = 1.15f;
		[SerializeField, Range (0f, 2f)] private float _raycastsDistance = 1.5f;
		[SerializeField, Range (0f, 1f)] private float _pelvisVerticalSpeed = 0.28f;
		[SerializeField, Range (0f, 1f)] private float _feetToIKPositionSpeed = 0.5f;
		[SerializeField] private float _pelvisOffset = 0f;
		[SerializeField] private LayerMask _environmentMask = 0;
		[Space (15)]
		[SerializeField] private bool _useProIKFeature = false;
		[SerializeField] private bool _showSolverDebug = true;

		private void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
		{
			Vector3 targetIKPosition = _animator.GetIKPosition(foot);
			if (positionIKHolder != Vector3.zero)
			{
				targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
				positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

				float y = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, _feetToIKPositionSpeed);
				targetIKPosition.y += y;
				lastFootPositionY = y;

				targetIKPosition = transform.TransformPoint(targetIKPosition);
				_animator.SetIKRotation(foot, rotationIKHolder);
			}

			_animator.SetIKPosition(foot, targetIKPosition);
		}

		private void MovePelvisHeight()
		{
			if (_rightFootIKPosition == Vector3.zero || _leftFootIKPosition == Vector3.zero || _lastPelvisPositionY == 0)
			{
				_lastPelvisPositionY = _animator.bodyPosition.y;
				return;
			}

			Vector3 position = transform.position;
			float leftOffsetPosition = _leftFootIKPosition.y - position.y;
			float rightOffsetPosition = _rightFootIKPosition.y - position.y;
			float totalOffset = leftOffsetPosition < rightOffsetPosition ? leftOffsetPosition : rightOffsetPosition;

			Vector3 newPelvisPosition = _animator.bodyPosition + Vector3.up * totalOffset;
			newPelvisPosition.y = Mathf.Lerp(_lastPelvisPositionY, newPelvisPosition.y, _pelvisVerticalSpeed);
			_animator.bodyPosition = newPelvisPosition;
			_lastPelvisPositionY = _animator.bodyPosition.y;
		}

		private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
		{
			if (_showSolverDebug)
				Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (_raycastsDistance + _heightFromGroundRaycast), Color.green);

			if (Physics.Raycast(fromSkyPosition, Vector3.down, out RaycastHit hit, _raycastsDistance + _heightFromGroundRaycast, _environmentMask))
			{
				feetIKPositions = fromSkyPosition;
				feetIKPositions.y = hit.point.y + _pelvisOffset;
				feetIKRotations = Quaternion.FromToRotation(Vector3.up, hit.normal);
				return;
			}

			feetIKPositions = Vector3.zero;
		}

		private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
		{
			feetPositions = _animator.GetBoneTransform(foot).position;
			feetPositions.y = transform.position.y + _heightFromGroundRaycast;
		}

		private void FixedUpdate()
		{
			if (!enableFeetIK)
				return;

			AdjustFeetTarget(ref _rightFootPosition, HumanBodyBones.RightFoot);
			AdjustFeetTarget(ref _leftFootPosition, HumanBodyBones.LeftFoot);
			FeetPositionSolver(_rightFootPosition, ref _rightFootIKPosition, ref _rightFootIKRotation);
			FeetPositionSolver(_leftFootPosition, ref _leftFootIKPosition, ref _leftFootIKRotation);
		}

		private void OnAnimatorIK(int layerIndex)
		{
			if (!enableFeetIK)
				return;

			MovePelvisHeight();

			_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
			if (_useProIKFeature)
				_animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _animator.GetFloat(RIGHT_FOOT_ANIM_VAR_NAME));

			MoveFeetToIKPoint(AvatarIKGoal.RightFoot, _rightFootIKPosition, _rightFootIKRotation, ref _lastRightFootPositionY);

			_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
			if (_useProIKFeature)
				_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat(LEFT_FOOT_ANIM_VAR_NAME));

			MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, _leftFootIKPosition, _leftFootIKRotation, ref _lastLeftFootPositionY);
		}
	}
}