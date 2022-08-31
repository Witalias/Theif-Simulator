using UnityEngine;
using System.Collections.Generic;
using System;

public class VisibilityEventsList : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private Sprite padlock;
    [SerializeField] private Sprite signal;
    [SerializeField] private Sprite speed;
    [SerializeField] private Sprite suitcase;
    [SerializeField] private Sprite trap;
    [SerializeField] private Sprite policeBadge;
    [SerializeField] private Sprite eye;

    [Header("from/to")]
    [SerializeField] private Vector2 lockedDoorsAndWindowsCount = new Vector2(3, 6);
    [SerializeField] private Vector2 furnitureWithTrapsCount = new Vector2(2, 4);

    private LevelGenerator generator;

    private List<VisibilityEvent> events;
    private List<VisibilityEvent> currentEvents;

    public Vector2 LockedDoorsAndWindowsCount { get => lockedDoorsAndWindowsCount; set => lockedDoorsAndWindowsCount = value; }

    public Vector2 FurnitureWithTrapsCount { get => furnitureWithTrapsCount; set => furnitureWithTrapsCount = value; }

    public void StartRandomEvent(out Sprite icon, out string message)
    {
        if (currentEvents.Count == 0)
        {
            icon = null;
            message = "";
            return;
        }
        var randomEvent = currentEvents[UnityEngine.Random.Range(0, currentEvents.Count)];
        icon = randomEvent.Icon;
        message = randomEvent.Message;
        randomEvent.Action?.Invoke();

        switch (randomEvent.Type)
        {
            case VisibilityEventType.MoreIncreasedSpeed:
                currentEvents.Add(new VisibilityEvent(VisibilityEventType.MoreIncreasedSpeed, speed, () =>
                    generator.IncreaseEnemiesSpeed(GameSettings.Instanse.IncreaseInResidentSpeed * 0.5f)));
                break;
        }
        currentEvents.Remove(randomEvent);
    }

    private void Start()
    {
        generator = GameObject.FindGameObjectWithTag(Tags.LevelGenerator.ToString()).GetComponent<LevelGenerator>();

        events = new List<VisibilityEvent>
        {
            new VisibilityEvent(VisibilityEventType.LockDoorsAndWindows, padlock, () =>
                generator.LockRandomDoorsAndWindows((int)UnityEngine.Random.Range(lockedDoorsAndWindowsCount.x, lockedDoorsAndWindowsCount.y))),
            new VisibilityEvent(VisibilityEventType.IncreasedRangeAndViewAngle, eye, () =>
            {
                generator.IncreaseEnemiesViewAngle(GameSettings.Instanse.IncreaseInResidentViewAngle);
                generator.IncreaseEnemiesViewDistance(GameSettings.Instanse.IncreaseInResidentViewDistance);
            }),
            new VisibilityEvent(VisibilityEventType.IncreasedSpeed, speed, () =>
                generator.IncreaseEnemiesSpeed(GameSettings.Instanse.IncreaseInResidentSpeed)),
            new VisibilityEvent(VisibilityEventType.DoubleLocks, padlock, () =>
                GameSettings.Instanse.DoubleLocks = true),
            new VisibilityEvent(VisibilityEventType.NoReactionToNoise, signal, () =>
                GameSettings.Instanse.NoResidentsReactionOnIntentionalNoise = true)
        };

        currentEvents = new List<VisibilityEvent>(events);
    }
}

public class VisibilityEvent
{
    public VisibilityEventType Type { get; }
    public string Message { get; }
    public Sprite Icon { get; }
    public Action Action { get; }

    public VisibilityEvent(VisibilityEventType type, Sprite icon, Action action)
    {
        Type = type;
        Message = Translation.Get(type);
        Icon = icon;
        Action = action;
    }
}
