using UnityEngine;

public class TemplarController : MonoBehaviour
{
    [SerializeField] private TemplarView _templarView = null;
    [SerializeField] private CameraController _cameraController = null;
    [SerializeField] private BoxCollider2D _boxCollider2D = null;
    [SerializeField] private TemplarControllerDatas _controllerDatas = null;
    [SerializeField] private AttackHitboxesContainer _attackHitboxesContainer = null;
    [SerializeField] private LayerMask _collisionMask = 0;

    private Vector3 _currVel;
    private Vector3 _prevVel;
    private float _refVelX;
    private float _jumpVel;

    public TemplarView TemplarView => _templarView;
    public CameraController CameraController => _cameraController;
    public TemplarControllerDatas ControllerDatas => _controllerDatas;
    public AttackHitboxesContainer AttackHitboxesContainer => _attackHitboxesContainer;

    public TemplarInputController InputCtrl { get; private set; }
    public TemplarJumpController JumpCtrl { get; private set; }
    public TemplarRollController RollCtrl { get; private set; }
    public TemplarAttackController AttackCtrl { get; private set; }
    public CollisionsController CollisionsCtrl { get; private set; }

    public float CurrDir { get; private set; }
    public float Gravity { get; private set; }

    public bool JumpAllowedThisFrame { get; private set; }

    public void Jump()
    {
        _currVel.y = _jumpVel;
    }

    public void Translate(Vector3 vel)
    {
        vel = CollisionsCtrl.ComputeCollisions(vel * Time.deltaTime);
        transform.Translate(vel);
    }

    private void OnCollisionDetected(CollisionsController.CollisionOrigin origin)
    {
        switch (origin)
        {
            case CollisionsController.CollisionOrigin.BELOW:
            {
                if (!InputCtrl.CheckInput(TemplarInputController.ButtonCategory.ROLL) && !RollCtrl.IsRolling && !CollisionsCtrl.PreviousStates.GetCollisionState(origin))
                {
                    UnityEngine.Assertions.Assert.IsTrue(_currVel.y < 0f, $"Detected a landing with a positive y velocity ({_currVel.y})!");

                    if (_controllerDatas.Jump.MinVelForLandImpact > -1 && -_currVel.y > _controllerDatas.Jump.MinVelForLandImpact)
                        JumpCtrl.TriggerLandImpact(-_currVel.y);
                    else
                        _templarView.PlayIdleAnimation(); // Landing with no speed impact.
                }

                break;
            }

            default:
                break;
        }
    }

    [ContextMenu("Compute Jump Physics")]
    private void ComputeJumpPhysics()
    {
        Gravity = -(2f * _controllerDatas.Jump.JumpHeight) / _controllerDatas.Jump.JumpApexDurSqr;
        _jumpVel = Mathf.Abs(Gravity) * _controllerDatas.Jump.JumpApexDur;
    }

    private void ResetVelocity()
    {
        _currVel = Vector3.zero;
    }

    private void TryRoll()
    {
        if (RollCtrl.IsRollingOrInCooldown
            || AttackCtrl.IsAttacking
            || !InputCtrl.CheckInput(TemplarInputController.ButtonCategory.ROLL)
            || !CollisionsCtrl.Below
            || JumpCtrl.IsInLandImpact)
            return;

        _currVel = Vector3.zero;
        InputCtrl.ResetDelayedInput(TemplarInputController.ButtonCategory.ROLL);
        RollCtrl.Roll(InputCtrl.Horizontal != 0f ? InputCtrl.Horizontal : CurrDir);
    }

    private void TryAttack()
    {
        if (RollCtrl.IsRolling
            || AttackCtrl.IsAttacking
            || !InputCtrl.CheckInput(TemplarInputController.ButtonCategory.ATTACK)
            || JumpCtrl.IsInLandImpact)
            return;

        InputCtrl.ResetDelayedInput(TemplarInputController.ButtonCategory.ATTACK);

        if (CollisionsCtrl.Below)
        {
            AttackCtrl.Attack((comboFinalDir) =>
            {
                CurrDir = comboFinalDir;
                if (AttackCtrl.CurrentAttackDatas.ControlVelocity)
                    ResetVelocity();
            });
        }
        else if (AttackCtrl.CanAttackAirborne)
        {
            AttackCtrl.AttackAirborne(() =>
            {
                if (AttackCtrl.CurrentAttackDatas.ControlVelocity)
                    ResetVelocity();
            });
        }
    }

    private void Move()
    {
        if (RollCtrl.IsRolling || AttackCtrl.IsAttacking && AttackCtrl.CurrentAttackDatas.ControlVelocity)
            return;

        if (CollisionsCtrl.Vertical && !JumpCtrl.IsAnticipatingJump)
        {
            _currVel.y = 0f;
            if (CollisionsCtrl.Below)
            {
                JumpCtrl.ResetJumpsLeft();
                AttackCtrl.ResetAirborneAttack();
            }
        }

        if (InputCtrl.Horizontal != 0f)
            CurrDir = InputCtrl.CurrentInputDir;

        // Jump.
        if (JumpCtrl.JumpsLeft > 0
            && InputCtrl.CheckInput(TemplarInputController.ButtonCategory.JUMP)
            && !JumpCtrl.IsInLandImpact && !JumpCtrl.IsAnticipatingJump
            && (AttackCtrl.CurrentAttackDatas == null || AttackCtrl.CanChainAttack))
        {
            JumpAllowedThisFrame = true;
            InputCtrl.ResetDelayedInput(TemplarInputController.ButtonCategory.JUMP);

            if (CollisionsCtrl.Below)
            {
                if (_controllerDatas.Jump.JumpAnticipationDur > 0)
                    JumpCtrl.JumpAfterAnticipation();
                else
                    Jump();
            }
            else
            {
                CProLogger.Log(this, "Double jump.", gameObject);
                JumpCtrl.JumpsLeft--;

                // Airborne jump.
                if (_controllerDatas.Jump.AirborneJumpAnticipationDur > 0)
                {
                    JumpCtrl.JumpAfterAnticipation(true);
                }
                else
                {
                    Jump();
                    _templarView.PlayDoubleJumpAnimation();
                }
            }
        }

        float targetVelX = _controllerDatas.RunSpeed;
        targetVelX *= InputCtrl.Horizontal;
        if (JumpCtrl.IsAnticipatingJump)
            targetVelX *= _controllerDatas.Jump.JumpAnticipationSpeedMult;
        if (JumpCtrl.IsInLandImpact)
            targetVelX *= JumpCtrl.LandImpactSpeedMult;

        float grav = Gravity * Time.deltaTime;
        if (_currVel.y < 0f)
            grav *= _controllerDatas.Jump.FallMultiplier;

        _currVel.x = Mathf.SmoothDamp(_currVel.x, targetVelX, ref _refVelX, CollisionsCtrl.Below ? _controllerDatas.GroundedDamping : _controllerDatas.Jump.AirborneDamping);
        _currVel.y += grav;

        Translate(_currVel);

        // Doing a grounded jump or falling will trigger this condition and remove one jump left. We need to do this after the ComputeCollisions call.
        if (!CollisionsCtrl.Below && CollisionsCtrl.PreviousStates.GetCollisionState(CollisionsController.CollisionOrigin.BELOW))
            JumpCtrl.JumpsLeft--;
    }

    private void BackupCurrentState()
    {
        CollisionsCtrl.BackupCurrentState();
        _prevVel = _currVel;
    }

    private void ResetCurrentState()
    {
        InputCtrl.Reset();
        JumpAllowedThisFrame = false;
    }

    private void Awake()
    {
        InputCtrl = new TemplarInputController(_controllerDatas.Input, this);
        JumpCtrl = new TemplarJumpController(this);
        RollCtrl = new TemplarRollController(this);
        AttackCtrl = new TemplarAttackController(this);
        CollisionsCtrl = new CollisionsController(_boxCollider2D, _collisionMask);
        CollisionsCtrl.CollisionDetected += OnCollisionDetected;
        _templarView.SetTemplarController(this);

        ComputeJumpPhysics();

        if (_controllerDatas.GroundOnAwake)
            CollisionsCtrl.Ground(transform);

        CurrDir = _templarView.GetSpriteRendererFlipX() ? -1f : 1f;
    }

    private void Update()
    {
        BackupCurrentState();
        ResetCurrentState();

        InputCtrl.Update();

        TryRoll();
        TryAttack();
        Move();

        CollisionsCtrl.TriggerDetectedCollisionsEvents();

        _templarView.UpdateView(
            flip: CurrDir != 1f,
            rolling: RollCtrl.IsRolling,
            attacking: AttackCtrl.IsAttacking,
            currVel: _currVel,
            prevVel: _prevVel);
    }
}