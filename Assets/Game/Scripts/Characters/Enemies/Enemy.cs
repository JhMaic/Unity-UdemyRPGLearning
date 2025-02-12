using CustomInspector;
using UnityEngine;

public class Enemy : Character
{
    [Title("Enemy")]
    [ForceFill] [SerializeField] private RayCast2D fallDetector;
    [ForceFill] [SerializeField] private RayCast2D playerDetector;
    [ForceFill] [SerializeField] private OverlapBox2D attackRange;
    [ForceFill] [SerializeField] private OverlapBox2D battleRange;
    protected bool IsGonnaFall => !fallDetector.IsHit();
    protected bool IsPlayerDetected => playerDetector.IsHit();
    protected bool IsPlayerInsideAttackRange => attackRange.IsOverlapped();
    protected bool IsPlayerInsideBattleRange => battleRange.IsOverlapped();
    // protected bool IsPlayerAttacked => attackHitBox.IsOverlapped();


    protected virtual void Move(float rate = 1)
    {
        rb.linearVelocityX = MoveSpeed * facingDir * rate;
    }

    protected abstract class EnemyState<T> : CharacterState<T> where T : Enemy
    {
        protected EnemyState(string animBoolName, T ctx) : base(animBoolName, ctx)
        {
        }
    }
}