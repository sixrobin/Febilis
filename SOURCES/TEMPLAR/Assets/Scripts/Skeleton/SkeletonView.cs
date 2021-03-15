using RSLib.Extensions;
using UnityEngine;

public class SkeletonView : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IDLE = "Idle";
    private const string HURT = "Hurt";
    private const string ATTACK = "Attack";
    private const string ATTACK_ANTICIPATION = "Attack_Anticipation";

    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private Transform _vfxScaler = null;
    [SerializeField] private ParticleSystem _hitVFX = null;
    [SerializeField] private RSLib.ImageEffects.SpriteBlink _spriteBlink = null;

    public SkeletonController SkeletonController { get; set; }

    public bool GetSpriteRendererFlipX()
    {
        return _spriteRenderer.flipX;
    }

    public void UpdateView(bool flip)
    {
        _animator.SetBool(IS_WALKING, SkeletonController.IsWalking);

        _spriteRenderer.flipX = flip;
        _vfxScaler.SetScaleX(flip ? -1f : 1f);
    }

    public void PlayIdleAnimation()
    {
        _animator.SetTrigger(IDLE);
    }

    public void PlayHurtAnimation()
    {
        _animator.SetTrigger(HURT);
        _hitVFX.Play();
    }

    public void PlayAttackAnticipationAnimation(string suffix = "")
    {
        _animator.SetTrigger($"{ATTACK_ANTICIPATION}{suffix}");
    }

    public void PlayAttackAnimation(string suffix = "")
    {
        _animator.SetTrigger($"{ATTACK}{suffix}");
        FindObjectOfType<TemplarCameraController>().Shake.SetTrauma(0.2f, 0.45f); // [TMP] GetComponent + hard coded values.
    }

    public void PlayDamageBlink()
    {
        _spriteBlink.BlinkColor();
    }

    public void ResetAttackTrigger()
    {
        _animator.ResetTrigger(ATTACK);
    }
}