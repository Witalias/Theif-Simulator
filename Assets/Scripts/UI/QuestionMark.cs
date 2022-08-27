using UnityEngine;

[RequireComponent(typeof(Animation))]
public class QuestionMark : MonoBehaviour
{
    private const string swingAnimationName = "Swing";

    private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void PlaySwing() => anim.Play(swingAnimationName);
}
