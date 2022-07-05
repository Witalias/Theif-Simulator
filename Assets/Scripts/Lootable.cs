using UnityEngine;

public class Lootable : MonoBehaviour
{
    private const float movingSpeed = 10f;

    [SerializeField] private Transform[] movingObjects;

    [Tooltip("Оставьте нули, если объект не нужно перемещать")]
    [SerializeField] private Vector3[] endLocalPositions;

    [Tooltip("Оставьте нули, если объект не нужно вращать")]
    [SerializeField] private Vector3[] endLocalRotations;

    private MovementController movementController;

    private bool moveChildObjects = false;
    private bool empty = false;

    public void TakeLoot()
    {
        empty = true;
        MoveChildObjects();

        var illumination = GetComponent<Illumination>();
        if (illumination != null)
        {
            illumination.RemoveIllumination();
            illumination.Enabled = false;
        }
    }

    private void Start()
    {
        movementController = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
    }

    private void Update()
    {
        if (moveChildObjects)
        {
            for (var i = 0; i < movingObjects.Length; ++i)
            {
                if (endLocalPositions[i] != Vector3.zero)
                    movingObjects[i].localPosition = Vector3.Lerp(movingObjects[i].localPosition, endLocalPositions[i], Time.deltaTime * movingSpeed);
                if (endLocalRotations[i] != Vector3.zero)
                    movingObjects[i].localRotation = Quaternion.Lerp(movingObjects[i].localRotation, Quaternion.Euler(endLocalRotations[i]), Time.deltaTime * movingSpeed);
            }
        }
    }

    private void OnMouseDown()
    {
        if (!empty)
        {
            var centeredPoint = GetComponent<CenteredPoint>();
            if (centeredPoint == null)
                movementController.GoToObject(transform.position, this);
            else
                movementController.GoToObject(centeredPoint.CenterPoint, this);
        }
        //if (!empty)
        //    TakeLoot();
    }

    private void MoveChildObjects()
    {
        if (movingObjects.Length == 0 || endLocalPositions.Length == 0 || endLocalRotations.Length == 0)
            return;

        moveChildObjects = true;
    }
}
