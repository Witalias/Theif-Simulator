using DG.Tweening;
using System;
using UnityEngine;

public class MovingFurnitureElements : MonoBehaviour
{
    [Serializable]
    private class MovingObject
    {
        public Transform Object;
        public bool ChangePosition;
        public Vector3 EndLocalPosition;
        public bool ChangeRotation;
        public Vector3 EndLocalRotation;

        public MovingObject(MovingObject other)
        {
            Object = other.Object;
            ChangePosition = other.ChangePosition;
            EndLocalPosition = other.EndLocalPosition;
            ChangeRotation = other.ChangeRotation;
            EndLocalRotation = other.EndLocalRotation;
        }
    }

    private const float ANIMATION_DURATION = 0.25f;

    [SerializeField] private MovingObject[] _movingObjects;

    private MovingObject[] _initialMovingStates;

    public void MoveForward()
    {
        Move(_movingObjects);
    }

    public void MoveBack()
    {
        Move(_initialMovingStates);
    }

    private void Awake()
    {
        _initialMovingStates = new MovingObject[_movingObjects.Length];
        for (var i = 0; i < _movingObjects.Length; i++)
        {
            _initialMovingStates[i] = new MovingObject(_movingObjects[i])
            {
                EndLocalPosition = _movingObjects[i].Object.localPosition,
                EndLocalRotation = _movingObjects[i].Object.localEulerAngles
            };
        }
    }

    private void Move(MovingObject[] moves)
    {
        foreach (var movable in moves)
        {
            if (movable.ChangePosition)
                movable.Object.DOLocalMove(movable.EndLocalPosition, ANIMATION_DURATION);
            if (movable.ChangeRotation)
                movable.Object.DOLocalRotate(movable.EndLocalRotation, ANIMATION_DURATION);
        }
    }
}
