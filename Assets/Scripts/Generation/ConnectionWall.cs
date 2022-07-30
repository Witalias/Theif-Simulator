using UnityEngine;

public class ConnectionWall : MonoBehaviour
{
    private const string openAnimatorBool = "Open";

    [SerializeField] private Transform passagePoint;
    [SerializeField] private Transform outsidePoint;
    [SerializeField] private GameObject door;

    private Animator doorAnimator;
    private LevelGenerator generator;
    private bool triggered = false;
    private bool fixedUpdateDone = false;

    public Vector3 PassagePointPosition { get => passagePoint.position; }

    public Vector3 OutsidePoint { get => outsidePoint.position; }

    public bool RemoveAfterGeneration { get; set; }

    public void RotateDoor(float value)
    {
        CheckPresenceDoorAnimator();
        doorAnimator.transform.localRotation = Quaternion.Euler(0, value, 0);
    }

    public void Delete()
    {
        if (RemoveAfterGeneration)
            Destroy(gameObject);
    }

    private void Start()
    {
        generator = GameObject.FindGameObjectWithTag(Tags.LevelGenerator.ToString()).GetComponent<LevelGenerator>();
    }

    private void FixedUpdate()
    {
        if (generator.Generated && !fixedUpdateDone)
        {
            //if (RemoveAfterGeneration)
            //{
            //    Destroy(gameObject);
            //    RemoveAfterGeneration = false;
            //}
            CheckPresenceDoorAnimator();
            doorAnimator.enabled = true;
            fixedUpdateDone = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag(Tags.Player.ToString()))
        {
            CheckPresenceDoorAnimator();

            triggered = true;
            doorAnimator.SetBool(openAnimatorBool, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggered && other.CompareTag(Tags.Player.ToString()))
        {
            triggered = false;
            doorAnimator.SetBool(openAnimatorBool, false);
        }
    }

    private void CheckPresenceDoorAnimator()
    {
        if (doorAnimator == null)
            doorAnimator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
    }
}
