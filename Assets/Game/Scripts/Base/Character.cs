using CustomInspector;
using Cysharp.Threading.Tasks;
using Game.Scripts.Structs;
using R3;
using UnityEngine;
using UnityEngine.Events;

public class Character : AnimatedEntity
{
    [Title("Character")]
    [SerializeField] [SelfFill(true)] protected Rigidbody2D rb;
    [SerializeField] [SelfFill(true, mode = OwnerMode.DirectChildren)]
    protected RectTransform rect;

    [SelfFill(true)]
    public CharacterStatAgent statAgent;

    [ForceFill] [SerializeField] private RayCast2D wallChecker;
    [ForceFill] [SerializeField] private LayerMask groundLayerMask;
    [ForceFill] [SerializeField] private Collider2D attackHitBox;
    [SerializeField] protected float hurtFrequency = 0.1f;
    [SerializeField] protected Material hurtFXMaterial;


    private ContactFilter2D _groundedContactFilter2D;

    private float _timeScale = 1;
    protected int facingDir = 1;
    protected float MoveSpeed => statAgent.moveSpeed.FinalValue;

    public UnityEvent<Damage> HurtEvent { get; } = new();


    protected UnityEvent<Collider2D> HitBoxEvents => attackHitBox.GetComponent<ColliderEvents>().TriggerEnter2DActions;
    protected bool IsGrounded => rb.IsTouching(_groundedContactFilter2D);
    protected bool IsWallDetected => wallChecker.IsHit();
    protected bool IsRising => rb.linearVelocityY > 0;


    /// <summary>
    /// </summary>
    /// <param name="scale">
    ///     1: normal
    ///     0: frozen
    /// </param>
    public virtual float TimeScale
    {
        get => _timeScale;
        set
        {
            var fixedValue = Mathf.Clamp01(value);
            _timeScale = fixedValue;
            anim.speed = fixedValue;
            statAgent.moveSpeed.AddMultModifier("timescale", value);
            statAgent.gravityScale.AddMultModifier("timescale", value);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _groundedContactFilter2D = new ContactFilter2D
        {
            useLayerMask = true,
            useNormalAngle = true,
            minNormalAngle = 80,
            maxNormalAngle = 100,
            layerMask = groundLayerMask
        };

        HurtEvent.AsObservable()
            .SubscribeAwait(async (damage, ct) =>
            {
                OnHurt(damage);
                await UniTask.WaitForSeconds(hurtFrequency, cancellationToken: ct);
            }, AwaitOperation.Drop)
            .AddTo(this);
    }

    protected virtual void Start()
    {
    }

    public virtual void Die()
    {
    }

    protected virtual void OnHurt(Damage damage)
    {
        statAgent.ApplyDamage(Mathf.FloorToInt(damage.value));
    }


    protected void FlipX()
    {
        facingDir *= -1;
        rb.transform.Rotate(0, 180, 0);
        rect.transform.eulerAngles = Vector3.zero;
    }

    protected void TurnLeft()
    {
        facingDir = -1;
        rb.transform.eulerAngles = new Vector3(0, 180, 0);
        rect.transform.eulerAngles = Vector3.zero;
    }

    protected void TurnRight()
    {
        facingDir = 1;
        rb.transform.eulerAngles = new Vector3(0, 0, 0);
        rect.transform.eulerAngles = Vector3.zero;
    }


    protected abstract class CharacterState<T> : AnimatedEntityState<T> where T : Character
    {
        protected CharacterState(string animBoolName, T ctx) : base(animBoolName, ctx)
        {
        }
    }
}