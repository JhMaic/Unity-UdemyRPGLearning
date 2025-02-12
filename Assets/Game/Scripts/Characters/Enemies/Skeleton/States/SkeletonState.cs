public partial class Skeleton
{
    protected abstract class SkeletonState : EnemyState<Skeleton>
    {
        protected SkeletonState(string animBoolName, Skeleton ctx) : base(animBoolName, ctx)
        {
        }
    }
}