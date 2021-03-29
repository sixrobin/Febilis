namespace Templar.Unit.Enemy
{
    public class EnemyView : UnitView
    {
        private const string HURT = "Hurt";
        private const string ATTACK = "Attack";
        private const string ATTACK_ANTICIPATION = "Attack_Anticipation";

        public void PlayAttackAnticipationAnimation(string suffix = "")
        {
            _animator.SetTrigger($"{ATTACK_ANTICIPATION}{suffix}");
        }

        public void PlayAttackAnimation(string suffix = "")
        {
            _animator.SetTrigger($"{ATTACK}{suffix}");
            FindObjectOfType<Templar.Camera.CameraController>().Shake.SetTrauma(0.2f, 0.45f); // [TMP] GetComponent + hard coded values.
        }
    }
}