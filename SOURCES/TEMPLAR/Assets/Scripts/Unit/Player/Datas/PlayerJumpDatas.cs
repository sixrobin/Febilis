namespace Templar.Unit.Player.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Player Jump Datas", menuName = "Datas/Player/Jump")]
    public class PlayerJumpDatas : ScriptableObject
    {
        [Header("JUMP")]
        [Tooltip("Height reached by the controller's pivot when at his jump apex.")]
        [SerializeField, Min(0f)] private float _jumpHeight = 2f;

        [Tooltip("Duration in seconds that the controller takes to reach his jump apex.")]
        [SerializeField, Min(0f)] private float _jumpApexDur = 0.5f;

        [Tooltip("Multiplier applied to Y velocity when controller is falling. Use 1 for no multiplying.")]
        [SerializeField, Min(0f)] private float _fallMultiplier = 1f;

        [Tooltip("Duration in seconds to reach target speed when controller is airborne. Use 0 for no damping.")]
        [SerializeField, Range(0f, 1f)] private float _airborneDamping = 0.2f;

        [Tooltip("Multiplier applied to X velocity when controller is preparing his jump. Use 1 for no multiplying.")]
        [SerializeField, Range(0f, 1f)] private float _jumpAnticipationSpeedMult = 0.6f;

        [Tooltip("Jump anticipation duration. Use 0 for an instantaneous jump.")]
        [SerializeField, Min(0f)] private float _jumpAnticipationDur = 0.1f;

        [Tooltip("Airborne jump anticipation duration. Use 0 for an instantaneous jump, which should be wanted for an airborne jump.")]
        [SerializeField, Min(0f)] private float _airborneJumpAnticipationDur = 0.1f;

        [Tooltip("How many times can the player jump before he touches the ground and resets the counter.")]
        [SerializeField, Min(1)] private int _maxFollowingJumps = 1;

        [Header("LAND")]
        [Tooltip("Minimum Y velocity required to apply a landing impact. Use -1 to never apply an impact.")]
        [SerializeField, Min(-1f)] private float _minVelForLandImpact = 8f;

        [Tooltip("Minimum multiplier that can be applied to X velocity on landing impact (meaning that 0 will nullify velocity on highest impact).")]
        [SerializeField, Min(0f)] private float _landImpactSpeedMultMin = 0.05f;

        [Tooltip("Multiplier applied to Y velocity to compute landing impact values (meaning that 0 will result in no impact).")]
        [SerializeField, Range(0f, 1f)] private float _landImpactDurFactor = 0.03f;

        [Tooltip("Minimum and maximum values for landing impact duration, also used as range for impact speed multiplier normalization.")]
        [SerializeField] private Vector2 _landImpactDurMinMax = Vector2.zero;

        // Jump.
        public float JumpHeight => _jumpHeight;
        public float JumpApexDur => _jumpApexDur;
        public float JumpApexDurSqr => _jumpApexDur * _jumpApexDur;
        public float FallMultiplier => _fallMultiplier;
        public float AirborneDamping => _airborneDamping;
        public float JumpAnticipationSpeedMult => _jumpAnticipationSpeedMult;
        public float JumpAnticipationDur => _jumpAnticipationDur;
        public float AirborneJumpAnticipationDur => _airborneJumpAnticipationDur;
        public int MaxFollowingJumps => _maxFollowingJumps;

        // Land.
        public float MinVelForLandImpact => _minVelForLandImpact;
        public float LandImpactSpeedMultMin => _landImpactSpeedMultMin;
        public float LandImpactDurFactor => _landImpactDurFactor;
        public float LandImpactDurMin => _landImpactDurMinMax.x;
        public float LandImpactDurMax => _landImpactDurMinMax.y;

        private void OnValidate()
        {
            _landImpactDurMinMax.x = Mathf.Max(_landImpactDurMinMax.x, 0f);
            _landImpactDurMinMax.y = Mathf.Max(_landImpactDurMinMax.x, _landImpactDurMinMax.y);
        }
    }
}