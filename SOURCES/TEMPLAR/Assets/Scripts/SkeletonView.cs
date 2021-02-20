using RSLib.Extensions;
using UnityEngine;

public class SkeletonView : MonoBehaviour
{
    private const string IDLE = "Idle";
    private const string HURT = "Hurt";

    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private Transform _vfxScaler = null;
    [SerializeField] private ParticleSystem _hitVFX = null;

    public void UpdateView(bool flip)
    {
        _spriteRenderer.flipX = flip;
        _vfxScaler.SetScaleX(flip ? -1f : 1f);
    }

    public void PlayHurtAnimation()
    {
        StartCoroutine(HurtAnimationCoroutine(0.25f));
        _hitVFX.Play();
    }

    private System.Collections.IEnumerator HurtAnimationCoroutine(float dur)
    {
        _animator.SetTrigger(HURT);
        yield return RSLib.Yield.SharedYields.WaitForSeconds(dur);
        _animator.SetTrigger(IDLE);
    }
}