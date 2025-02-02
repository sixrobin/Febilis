﻿namespace Templar.Datas.Unit
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Player Roll Datas", menuName = "Datas/Player/Roll")]
    public class UnitRollDatas : ScriptableObject
    {
        [Header("ROLL")]
        [Tooltip("Full roll motion duration.")]
        [SerializeField, Min(0f)] private float _dur = 1f;

        [Tooltip("Speed that will be multiplied by the roll curve evaluation.")]
        [SerializeField, Min(0f)] private float _speed = 3f;

        [Tooltip("Percentage of the roll duration from which edges are detected not to fall.")]
        [SerializeField, Range(0f, 1f)] private float _edgeDetectionThreshold = 0f;

        [Tooltip("Curve that will be applied to roll speed over the roll duration. Values should be between 0 and 1.")]
        [SerializeField] private AnimationCurve _speedCurve = null;

        [Tooltip("Roll cooldown in seconds. Use 0 for no cooldown.")]
        [SerializeField, Min(0f)] private float _cooldown = 0.1f;

        [Tooltip("Multiplier applied to gravity while controller is rolling airborne.")]
        [SerializeField, Min(0f)] private AnimationCurve _gravityMultCurve = RSLib.AnimationCurves.One;

        [Tooltip("Multiplier applied to roll animation speed.")]
        [SerializeField] private float _animMult = 1f;

        public float Dur => _dur;
        public float Speed => _speed;
        public float EdgeDetectionThreshold => _edgeDetectionThreshold;
        public AnimationCurve SpeedCurve => _speedCurve;
        public float Cooldown => _cooldown;
        public bool HasCooldown => Cooldown > 0;
        public AnimationCurve GravityMultCurve => _gravityMultCurve;
        public float AnimMult => _animMult;
    }
}