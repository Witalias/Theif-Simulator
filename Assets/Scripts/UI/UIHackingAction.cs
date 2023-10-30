using UnityEngine;

[RequireComponent(typeof(Animation))]
public class UIHackingAction : MonoBehaviour
{
    [SerializeField] private GameObject _content;

    private Animation _animation;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    private void OnEnable()
    {
        WaitingAndAction.TimerActived += SetActive;
    }

    private void OnDisable()
    {
        WaitingAndAction.TimerActived -= SetActive;
    }

    private void SetActive(bool value)
    {
        _content.SetActive(value);

        if (value)
            _animation.Play();
    }
}
