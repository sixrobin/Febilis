using RSLib.Extensions;
using UnityEngine;

public class TemplarView : MonoBehaviour
{
    private const string IS_RUNNING = "IsRunning";
    private const string FALL = "Fall";
    private const string IDLE = "Idle";
    private const string JUMP = "Jump";
    private const string LAND = "Land";
    private const string ROLL = "Roll";
    private const string ATTACK = "Attack";
    private const string ATTACK_CHAIN = "AttackChain";
    private const string ATTACK_AIRBORNE = "AttackAirborne";
    private const string HURT = "Hurt";
    private const string DEATH = "Death";
    private const string MULT_ROLL = "Mult_Roll";
    private const string MULT_ATTACK = "Mult_Attack";

    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private RSLib.ImageEffects.SpriteBlink _spriteBlink = null;

    [Header("MOTION")]
    [SerializeField] private GameObject _jumpPuffPrefab = null;
    [SerializeField] private GameObject _doubleJumpPuffPrefab = null;
    [SerializeField] private GameObject _landPuffPrefab = null;
    [SerializeField] private GameObject _rollPuffPrefab = null;
    [SerializeField, Min(0f)] private float _landPuffMinVel = 5f;

    [Header("FIGHT")]
    [SerializeField] private GameObject _hitPrefab = null;
    [SerializeField] private Transform _hitVFXPivot = null;
    [SerializeField] private GameObject _attackPuffPrefab = null;
    [SerializeField] private GameObject[] _hurtPrefabs = null;

    public TemplarController TemplarController { get; set; }

    public bool GetSpriteRendererFlipX()
    {
        return _spriteRenderer.flipX;
    }

    public void UpdateView(bool flip, Vector3 currVel, Vector3 prevVel)
    {
        _animator.SetBool(IS_RUNNING, !TemplarController.IsBeingHurt && !TemplarController.RollCtrl.IsRolling && TemplarController.InputCtrl.Horizontal != 0f);

        if (!TemplarController.RollCtrl.IsRolling && !TemplarController.AttackCtrl.IsAttacking)
            _spriteRenderer.flipX = flip;

        if (currVel.y < 0f
            && (prevVel.y > 0f
            || TemplarController.CollisionsCtrl.PreviousStates.GetCollisionState(CollisionsController.CollisionOrigin.BELOW)
            && !TemplarController.CollisionsCtrl.Below)
            && !TemplarController.AttackCtrl.IsAttacking
            && !TemplarController.IsBeingHurt)
            _animator.SetTrigger(FALL);
    }

    public void PlayIdleAnimation()
    {
        _animator.SetTrigger(IDLE);
    }

    public void PlayJumpAnimation(float dir)
    {
        _animator.SetTrigger(JUMP);

        if (dir != 0f)
        {
            GameObject jumpPuffInstance = Instantiate(_jumpPuffPrefab, transform.position, _jumpPuffPrefab.transform.rotation);
            jumpPuffInstance.transform.SetScaleX(dir);
        }
        else
        {
            // We might probably want to use another prefab to have another VFX.s
            Instantiate(_landPuffPrefab, transform.position, _landPuffPrefab.transform.rotation);
        }
    }

    public void PlayDoubleJumpAnimation()
    {
        _animator.SetTrigger(JUMP);
        Instantiate(_doubleJumpPuffPrefab, transform.position, _doubleJumpPuffPrefab.transform.rotation);
    }

    public void PlayLandAnimation(float velYAbs)
    {
        _animator.SetTrigger(LAND);
        if (velYAbs > _landPuffMinVel)
            PlayLandVFX();
    }

    public void PlayLandVFX()
    {
        Instantiate(_landPuffPrefab, transform.position, _landPuffPrefab.transform.rotation);
    }

    public void PlayRollAnimation(float dir)
    {
        _spriteRenderer.flipX = dir < 0f;
        _animator.SetTrigger(ROLL);
        _animator.SetFloat(MULT_ROLL, TemplarController.ControllerDatas.Roll.AnimMult);

        GameObject rollPuffInstance = Instantiate(_rollPuffPrefab, transform.position, _rollPuffPrefab.transform.rotation);
        rollPuffInstance.transform.SetScaleX(dir);
    }

    public void PlayAttackAnimation(float dir)
    {
        UpdateAttackAnimation(dir);
        _animator.SetTrigger(ATTACK);
    }

    public void PlayChainAttackAnimation(float dir)
    {
        UpdateAttackAnimation(dir);
        _animator.SetTrigger(ATTACK_CHAIN);
    }

    public void PlayAttackAirborneAnimation()
    {
        UpdateAttackAnimation();
        _animator.SetTrigger(ATTACK_AIRBORNE);
    }

    public void PlayAttackVFX(float dir, float offset)
    {
        GameObject smallPuffInstance = Instantiate(_attackPuffPrefab, transform.position - new Vector3(dir * offset, 0f), _attackPuffPrefab.transform.rotation);
        smallPuffInstance.transform.SetScaleX(dir);
    }

    public void PlayHitVFX(float dir)
    {
        GameObject hitInstance = Instantiate(_hitPrefab, _hitVFXPivot.position, _hitPrefab.transform.rotation);
        hitInstance.transform.SetScaleX(dir);
    }

    public void PlayHurtAnimation(float dir)
    {
        _animator.SetTrigger(HURT);
        _spriteBlink.BlinkColor();

        for (int i = _hurtPrefabs.Length - 1; i >= 0; --i)
            Instantiate(_hurtPrefabs[i], transform.position, _hurtPrefabs[i].transform.rotation);

        // [TMP] We probably want a puff VFX made especially for hurt feedback.
        GameObject jumpPuffInstance = Instantiate(_jumpPuffPrefab, transform.position, _jumpPuffPrefab.transform.rotation);
        jumpPuffInstance.transform.SetScaleX(dir);
    }

    public void PlayDeathAnimation()
    {
        _animator.SetTrigger(DEATH);
        _spriteBlink.BlinkColor();

        for (int i = _hurtPrefabs.Length - 1; i >= 0; --i)
            Instantiate(_hurtPrefabs[i], transform.position, _hurtPrefabs[i].transform.rotation);
    }

    public void DBG_Color(Color col)
    {
        _spriteRenderer.color = col;
    }

    private void UpdateAttackAnimation(float dir = 0f)
    {
        _animator.SetFloat(MULT_ATTACK, TemplarController.AttackCtrl.CurrentAttackDatas.AnimMult);
        if (dir != 0f)
            _spriteRenderer.flipX = dir < 0f;
    }
}