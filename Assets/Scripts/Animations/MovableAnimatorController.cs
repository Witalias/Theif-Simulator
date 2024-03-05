using UnityEngine;

public class MovableAnimatorController : MonoBehaviour
{
    private int WALK_BOOLEAN = Animator.StringToHash("Walk");
    private int RUN_BOOLEAN = Animator.StringToHash("Run");

    [SerializeField] protected Animator _animator;

    public void WalkBoolean(bool value) => _animator.SetBool(WALK_BOOLEAN, value);

    public void RunBoolean(bool value) => _animator.SetBool(RUN_BOOLEAN, value);
}
