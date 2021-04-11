namespace Templar.Unit.Enemy
{
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyView : UnitView
    {
        public const float DEAD_FADE_DELAY = 0.8f;

        private const string ATTACK_ANM_OVERRIDE_ID = "Attack";
        private const string ATTACK_ANTICIPATION_ANM_OVERRIDE_ID = "Attack Anticipation";
        private const string ATTACK_CLIP_NAME_FORMAT = "Anm_{0}_Attack_{1}";
        private const string ATTACK_ANTICIPATION_CLIP_NAME_FORMAT = "Anm_{0}_Attack_{1}_Anticipation";

        private const string IS_WALKING = "IsWalking";
        private const string HURT = "Hurt";
        private const string ATTACK = "Attack";
        private const string ATTACK_ANTICIPATION = "Attack_Anticipation";

        [SerializeField] private AnimatorOverrideController _aocTemplate = null;

        private AnimatorOverrideController _aoc;
        private List<KeyValuePair<AnimationClip, AnimationClip>> _initClips;

        public string EnemyId { get; private set; }

        public void SetEnemyId(string id)
        {
            EnemyId = id;
        }

        public void PlayWalkAnimation(bool state)
        {
            _animator.SetBool(IS_WALKING, state);
        }

        public void SetupAttackOverrideClips(string id)
        {
            string attackClipName = string.Format(ATTACK_CLIP_NAME_FORMAT, EnemyId, id);
            string attackAnticipationClipName = string.Format(ATTACK_ANTICIPATION_CLIP_NAME_FORMAT, EnemyId, id);

            UnityEngine.Assertions.Assert.IsTrue(Datas.Unit.Enemy.EnemyDatabase.AnimationClips.ContainsKey(attackClipName), $"Animation clip {attackClipName} was not found in EnemyDatabase.");
            UnityEngine.Assertions.Assert.IsTrue(Datas.Unit.Enemy.EnemyDatabase.AnimationClips.ContainsKey(attackAnticipationClipName), $"Animation clip {attackAnticipationClipName} was not found in EnemyDatabase.");

            OverrideClip(ATTACK_ANM_OVERRIDE_ID, Datas.Unit.Enemy.EnemyDatabase.AnimationClips[attackClipName]);
            OverrideClip(ATTACK_ANTICIPATION_ANM_OVERRIDE_ID, Datas.Unit.Enemy.EnemyDatabase.AnimationClips[attackAnticipationClipName]);
        }

        public void PlayAttackAnticipationAnimation()
        {
            _animator.SetTrigger(ATTACK_ANTICIPATION);
        }

        public void PlayAttackAnimation()
        {
            _animator.SetTrigger(ATTACK);
            FindObjectOfType<Templar.Camera.CameraController>().Shake.SetTrauma(0.2f, 0.45f); // [TMP] GetComponent + hard coded values.
        }

        private void OverrideClip(string key, AnimationClip clip)
        {
            _aoc[key] = clip;
        }

        private void RestoreInitClip(string key)
        {
            foreach (KeyValuePair<AnimationClip, AnimationClip> initClip in _initClips)
                if (initClip.Key.name == key)
                    _aoc[key] = initClip.Value;
        }

        private void InitAnimatorOverrideController()
        {
            _aoc = new AnimatorOverrideController(_aocTemplate.runtimeAnimatorController) { name = "aocCopy" };

            List<KeyValuePair<AnimationClip, AnimationClip>> clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            _aocTemplate.GetOverrides(clips);
            _aoc.ApplyOverrides(clips);
            _initClips = clips;

            _animator.runtimeAnimatorController = _aoc;
        }

        private void Awake()
        {
            InitAnimatorOverrideController();
        }
    }
}