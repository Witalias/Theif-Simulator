using UnityEngine;

public class MovingFurnitureElements : MonoBehaviour
{
    private const float movingSpeed = 10f;

    [Tooltip("Оставьте пустым, если объект не будет заменён другим после обыска")]
    [SerializeField] private GameObject objectAfterLooting;

    [SerializeField] private Transform[] movingObjects;

    [Tooltip("Оставьте нули, если объект не нужно перемещать")]
    [SerializeField] private Vector3[] endLocalPositions;

    [Tooltip("Оставьте нули, если объект не нужно вращать")]
    [SerializeField] private Vector3[] endLocalRotations;

    private bool moveChildObjects = false;

    public void Move()
    {
        if (objectAfterLooting == null)
            MoveChildObjects();
        else
            ReplaceObject();
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

    private void MoveChildObjects()
    {
        if (movingObjects.Length == 0 || endLocalPositions.Length == 0 || endLocalRotations.Length == 0)
            return;

        moveChildObjects = true;
    }

    private void ReplaceObject()
    {
        Instantiate(objectAfterLooting, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }
}
