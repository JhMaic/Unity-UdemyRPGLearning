using CustomInspector;
using Game.Scripts.FSM;
using UnityEngine;

public class AnimatedEntity : MonoBehaviour
{
    [SelfFill(true, mode = OwnerMode.DirectChildren)]
    [SerializeField] protected SpriteRenderer sprite;

    [SerializeField] [ReadOnly] protected float stateTimer;

    [SelfFill(true, mode = OwnerMode.DirectChildren)]
    [SerializeField] protected Animator anim;

    protected AnimatorControl animatorController;


    /// <summary>
    ///     控制一部分一次性动画的标志位
    /// </summary>
    protected bool isAnimationFinished;

    protected virtual void Awake()
    {
        animatorController = GetComponentInChildren<AnimatorControl>();
        animatorController.AnimFinishedEvent += OnAnimFinished;
    }

    protected virtual void Update()
    {
        if (stateTimer > 0)
            stateTimer = Mathf.Max(0, stateTimer - Time.deltaTime);
    }

    private void OnDestroy()
    {
        animatorController.AnimFinishedEvent -= OnAnimFinished;
    }

    private void OnAnimFinished()
    {
        isAnimationFinished = true;
    }

    protected abstract class AnimatedEntityState<T> : State<T> where T : AnimatedEntity
    {
        private readonly string _animBoolName;

        protected AnimatedEntityState(string animBoolName, T ctx) : base(ctx)
        {
            _animBoolName = animBoolName;
        }

        public override void Enter()
        {
            base.Enter();
            ctx.anim.SetBool(_animBoolName, true);
            ctx.isAnimationFinished = false;
        }

        public override void Exit()
        {
            base.Exit();
            ctx.anim.SetBool(_animBoolName, false);
            ctx.isAnimationFinished = true;
        }
    }
}