using System;
using CustomInspector;
using R3;
using UnityEngine;

public partial class Sword_Controller : MonoBehaviour
{
    private Animator _anim;

    private bool _isReturning;

    private Player _player;
    private Rigidbody2D _rb;
    private ISwordController _swordController;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _player = PlayerManager.Instance.player;
    }

    private void Update()
    {
        _swordController?.Update();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        _swordController.OnDrawGizmos();
    }
#endif

    private void OnTriggerEnter2D(Collider2D other)
    {
        _swordController?.OnTriggerEnter2D(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        _swordController?.OnTriggerStay2D(other);
    }


    public void Launch()
    {
        _swordController?.Launch();
    }


    public void Setup<TConfig>(TConfig config) where TConfig : BaseConfig
    {
        _rb.gravityScale = config.launchGravityScale;

        if (config is DefaultConfig defaultConfig)
            _swordController = new DefaultType(this, defaultConfig);

        else if (config is BounceConfig bounceConfig)
            _swordController = new BounceType(this, bounceConfig);

        else if (config is PierceConfig pierceConfig)
            _swordController = new PierceType(this, pierceConfig);

        else if (config is SpinConfig spinConfig)
            _swordController = new SpinType(this, spinConfig);
    }


    public void ReturnToPlayer(float returnSpeed, float minCompleteDistance, Action onCompleted)
    {
        if (_isReturning || !_swordController.CanReturn())
            return;

        _isReturning = true;
        transform.parent = null;
        Observable.EveryUpdate()
            .Select(_ => _player.transform.position)
            .TakeWhile(playerPos => Vector2.Distance(playerPos, transform.position) > minCompleteDistance)
            .Subscribe(playerPos =>
            {
                var newPos = Vector2.MoveTowards(transform.position, playerPos, returnSpeed * Time.deltaTime);

                // setting sword sprite direction
                transform.right = (Vector2)transform.position - newPos;
                transform.position = newPos;
            }, _ =>
            {
                _isReturning = false;
                onCompleted();
            }).AddTo(this);
    }


    [Serializable]
    public record BaseConfig
    {
        [HideField] public Vector2 launchStartPos;
        [HideField] public Vector2 launchVelocity;
        public float launchGravityScale;
    }


    private abstract class SwordControlType<TConfig> : ISwordController where TConfig : BaseConfig
    {
        protected readonly TConfig config;
        protected readonly Sword_Controller ctx;

        public SwordControlType(Sword_Controller ctx, TConfig config)
        {
            this.ctx = ctx;
            this.config = config;
        }

        public virtual void OnTriggerStay2D(Collider2D other)
        {
        }

        public abstract void OnTriggerEnter2D(Collider2D other);
        public abstract void Update();

        /// <summary>
        ///     the first time pressed the launch key
        /// </summary>
        public virtual void Launch()
        {
            ctx.transform.position = config.launchStartPos;
            ctx._rb.gravityScale = config.launchGravityScale;
            ctx._rb.linearVelocity = config.launchVelocity;
        }


        public virtual void OnDrawGizmos()
        {
        }

        public virtual bool CanReturn()
        {
            return true;
        }
    }

    private interface ISwordController
    {
        public void OnTriggerEnter2D(Collider2D other);
        public void OnTriggerStay2D(Collider2D other);
        public void Update();

        /// <summary>
        ///     the first time pressed the launch key
        /// </summary>
        public void Launch();

        public void OnDrawGizmos();
        public bool CanReturn();
    }
}