using System;
using System.Collections.Generic;
using CustomInspector;
using Game.Scripts.FSM;
using Game.Scripts.Structs;
using UnityEngine;

public partial class Skeleton : Enemy
{
    public enum States
    {
        Idle,
        Move,
        Battle,
        Attack,
        Cooldown,
        Hurt,
        CounterAttacked,
        Blank
    }

    [HorizontalLine("Stat")]
    [SerializeField] private float maxStandbyTime = 5;
    [SerializeField] private float maxMoveTime = 5;
    [SerializeField] private float attackCooldownTime = 0.5f;
    [SerializeField] private float battleSpeedRate = 1.5f;
    [SerializeField] private float battleTime = 15f;

    [HorizontalLine("State Machine")]
    [SerializeField] private StateMachine<Skeleton> stateMachine;

    private CounterAttackSignal _counterAttackSignal;
    private Player _player;

    private bool IsCounterAttackAble => _counterAttackSignal.counterAttackAble;

    public override float TimeScale
    {
        set
        {
            base.TimeScale = value;
            stateMachine.CanChangeState = !TimeScale.Equals(0);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        var states = new Dictionary<Enum, State<Skeleton>>
        {
            { States.Idle, new IdleState("Idle", this) },
            { States.Move, new MoveState("Move", this) },
            { States.Battle, new BattleState("Move", this) },
            { States.Attack, new AttackState("Attack", this) },
            { States.Cooldown, new CooldownState("Idle", this) },
            { States.Hurt, new HurtState("Hurt", this) },
            { States.CounterAttacked, new CounterAttackedState("CounterAttacked", this) },
            { States.Blank, new BlankState(this) }
        };
        stateMachine = new StateMachine<Skeleton>(states, States.Idle);

        _counterAttackSignal = GetComponentInChildren<CounterAttackSignal>();
    }

    protected override void Start()
    {
        base.Start();
        _player = PlayerManager.Instance.player;
        stateMachine.Start();
    }


    protected override void Update()
    {
        base.Update();
        stateMachine.Update();
    }


    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(States.Blank);

        rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        GetComponent<Collider2D>().enabled = false;
    }

    protected override void OnHurt(Damage damage)
    {
        if (!stateMachine.CurrentState.Equals(States.Blank))
        {
            stateMachine.ChangeState(States.Hurt, damage);
            base.OnHurt(damage);
        }
    }
}