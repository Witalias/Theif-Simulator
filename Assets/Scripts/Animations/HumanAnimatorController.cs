using UnityEngine;

public class HumanAnimatorController : MovableAnimatorController
{
    private int SNEAK_BOOLEAN = Animator.StringToHash("Sneak");
    private int TAKE_BOOLEAN = Animator.StringToHash("Hack");
    private int SIT_BOOLEAN = Animator.StringToHash("Sit");
    private int SCARY_TRIGGER = Animator.StringToHash("Scary");

    public void SneakBoolean(bool value) => _animator.SetBool(SNEAK_BOOLEAN, value);

    public void TakeBoolean(bool value) => _animator.SetBool(TAKE_BOOLEAN, value);

    public void SitBoolean(bool value) => _animator.SetBool(SIT_BOOLEAN, value);

    public void ScaryTrigger() => _animator.SetTrigger(SCARY_TRIGGER);
}
