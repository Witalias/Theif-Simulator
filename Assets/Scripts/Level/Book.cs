using UnityEngine;

[RequireComponent(typeof(CenteredPoint))]
[RequireComponent(typeof(TargetObject))]
[RequireComponent(typeof(AudioSource))]
public class Book : MonoBehaviour
{
    [SerializeField] private float readingTime = 10f;

    private WaitingAndAction waitingAndAction;
    private CenteredPoint centeredPoint;
    private MovementController movementController;
    private TargetObject targetObject;
    private AudioSource audioSource;

    public void Read()
    {
        void ActionAfter()
        {
            Skills.Instanse.UseRandom();
            Destroy(gameObject);
        }
        SoundManager.Instanse.PlayLoop(Sound.BookFlip, audioSource);
        waitingAndAction.WaitAndExecuteWithSound(readingTime, ActionAfter, null, Sound.BookFlip);
    }

    private void Awake()
    {
        centeredPoint = GetComponent<CenteredPoint>();
        targetObject = GetComponent<TargetObject>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        waitingAndAction = GameObject.FindGameObjectWithTag(Tags.TimeCircle.ToString()).GetComponent<WaitingAndAction>();
        movementController = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
    }

    private void OnMouseDown()
    {
        if (Physics.Raycast(centeredPoint.CenterPoint, movementController.CenterPoint.position - centeredPoint.CenterPoint, out RaycastHit hit))
        {
            if (centeredPoint == null)
                movementController.GoToObject(transform.position, targetObject, hit);
            else
                movementController.GoToObject(centeredPoint.CenterPoint, targetObject, hit);
        }
    }
}
