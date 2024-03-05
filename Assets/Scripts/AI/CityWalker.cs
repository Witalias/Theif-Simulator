using UnityEngine;
using UnityEngine.AI;

public class CityWalker : PathTrajectory
{
    [SerializeField] private HumanAnimatorController _animatorController;

    public override void FollowTrajectory()
    {
        _animatorController.WalkBoolean(true);
        base.FollowTrajectory();
    }

    protected override void Stop()
    {
        _animatorController.WalkBoolean(false);
        base.Stop();
    }
}
