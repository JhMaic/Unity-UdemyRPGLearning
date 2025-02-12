using System;
using System.Collections.Generic;
using CustomInspector;
using Cysharp.Threading.Tasks;
using Game.Scripts.FSM;
using Game.Scripts.Structs;
using PrimeTween;
using UnityEngine;

public partial class Player : Character
{
    public enum States
    {
        Idle,
        Move,
        Air,
        Dash,
        WallSlide,
        WallJump,
        PrimaryAttack,
        Block,
        CounterAttack,
        AimSwordState,
        CatchSwordState,
        DeadState
    }

    [HorizontalLine("Player")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float comboDuration;
    [SerializeField] private float blockTime;

    [SerializeField] private TweenSettings<float> tweenCounterAttackMovement;

    [Header("Attack")]
    [SerializeField] private Vector2[] attackMovement;
    [SerializeField] private Vector2 primaryAttackKnockbackForce = new(3, 0);
    [SerializeField] private Vector2 counterAttackKnockbackForce = new(10, 0);

    [HorizontalLine("State Machine")]
    [SerializeField] private StateMachine<Player> stateMachine;

    private float _xInput;

    public int ComboCounter { get; private set; }

    public States CurrentState => (States)stateMachine.CurrentState;
    private SkillManager Skills => SkillManager.Instance;

    protected override void Awake()
    {
        base.Awake();
        var states = new Dictionary<Enum, State<Player>>
        {
            { States.Idle, new IdleState("Idle", this) },
            { States.Move, new MoveState("Move", this) },
            { States.Air, new AirState("Air", this) },
            { States.Dash, new DashState("Dash", this) },
            { States.WallSlide, new WallSlide("WallSlide", this) },
            { States.WallJump, new WallJump("Air", this) },
            { States.PrimaryAttack, new PrimaryAttack("Attack", comboDuration, this) },
            { States.Block, new BlockState("Block", this) },
            { States.CounterAttack, new CounterAttackState("CounterAttack", this) },
            { States.AimSwordState, new AimSwordState("AimSword", this) },
            { States.CatchSwordState, new CatchSwordState("CatchSword", this) },
            { States.DeadState, new DeadState("Die", this) }
        };
        stateMachine = new StateMachine<Player>(states, States.Idle);

        // 受伤效果
        // TODO: 或者减少hp之后才应用
        // if (hurtFXMaterial)
        //     HurtEvent.AsObservable()
        //         .SubscribeAwait(ApplyHurtFX, AwaitOperation.Drop)
        //         .AddTo(this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Start();
    }

    protected override void Update()
    {
        base.Update();
        _xInput = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("yVelocity", rb.linearVelocityY);
        stateMachine.Update();
    }


    protected override void OnHurt(Damage damage)
    {
        // 判断反击状态
        if (stateMachine.CurrentState.Equals(States.Block))
        {
            Debug.Log("Blocked!!");
            stateMachine.ChangeState(States.CounterAttack);
            return;
        }

        base.OnHurt(damage);
        HurtEffect();
    }

    /// <summary>
    ///     响应键盘的左右输入
    /// </summary>
    private void MoveInputHandle()
    {
        rb.linearVelocityX = _xInput * MoveSpeed;
    }

    /// <summary>
    ///     跳跃键响应
    /// </summary>
    private void JumpHandle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            rb.linearVelocityY = jumpForce;
    }


    /// <summary>
    ///     根据速度方向自动翻转
    /// </summary>
    private void FlipByVelocityX()
    {
        if (rb.linearVelocityX > 0 && facingDir.Equals(-1))
            TurnRight();
        else if (rb.linearVelocityX < 0 && facingDir.Equals(1))
            TurnLeft();
    }

    private async UniTask HurtEffect()
    {
        var material0 = sprite.material;

        if (!(stateMachine.CurrentState.Equals(States.CounterAttack) || stateMachine.CurrentState.Equals(States.Block)))
            sprite.material = hurtFXMaterial;

        await UniTask.WaitForSeconds(0.1f);
        sprite.material = material0;
    }

    public void CatchSword()
    {
        stateMachine.ChangeState(States.CatchSwordState);
    }

    public void Freeze()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
    }

    public void UnFreeze()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(States.DeadState);
    }
}