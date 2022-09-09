using UnityEngine;
using System.Collections.Generic;
using System;

public class VisibilityEventsList : MonoBehaviour
{
    [SerializeField] private int[] visibilityLevelsForPoliceman;

    [Header("from/to")]
    [SerializeField] private Vector2 lockedDoorsAndWindowsCount = new Vector2(3, 6);
    [SerializeField] private Vector2 furnitureWithTrapsCount = new Vector2(2, 4);

    [Header("Colors")]
    [SerializeField] private Color titleMessageColor;
    [SerializeField] private Color backgroundMessageColor;

    private LevelGenerator generator;
    private MessageQueue messageQueue;

    private List<VisibilityEvent> events;
    private List<VisibilityEvent> currentEvents;
    private VisibilityEvent callPoliceEvent;
    private VisibilityEvent newPolicemanEvent;
    private bool policeCalled = false;

    public Vector2 LockedDoorsAndWindowsCount { get => lockedDoorsAndWindowsCount; set => lockedDoorsAndWindowsCount = value; }

    public Vector2 FurnitureWithTrapsCount { get => furnitureWithTrapsCount; set => furnitureWithTrapsCount = value; }

    public void StartEvent(int visibilityLevel)
    {
        if (Array.Exists(visibilityLevelsForPoliceman, element => element == visibilityLevel))
        {
            CallPolice(visibilityLevel);
            return;
        }

        if (currentEvents.Count == 0)
            return;

        var randomEvent = currentEvents[UnityEngine.Random.Range(0, currentEvents.Count)];
        ExecuteEvent(randomEvent, visibilityLevel);

        var storage = GameStorage.Instanse;
        switch (randomEvent.Type)
        {
            case VisibilityEventType.IncreasedSpeed:
                currentEvents.Add(new VisibilityEvent(VisibilityEventType.MoreIncreasedSpeed, storage.Speed, () =>
                    generator.IncreaseEnemiesSpeed(GameSettings.Instanse.IncreaseInResidentSpeed * 0.5f)));
                break;
        }
        currentEvents.Remove(randomEvent);
    }

    private void Start()
    {
        generator = GameObject.FindGameObjectWithTag(Tags.LevelGenerator.ToString()).GetComponent<LevelGenerator>();
        messageQueue = GameObject.FindGameObjectWithTag(Tags.MessageQueue.ToString()).GetComponent<MessageQueue>();

        var storage = GameStorage.Instanse;
        events = new List<VisibilityEvent>
        {
            new VisibilityEvent(VisibilityEventType.LockDoorsAndWindows, storage.Padlock, () =>
                generator.LockRandomDoorsAndWindows((int)UnityEngine.Random.Range(lockedDoorsAndWindowsCount.x, lockedDoorsAndWindowsCount.y))),
            new VisibilityEvent(VisibilityEventType.IncreasedRangeAndViewAngle, storage.Eye, () =>
            {
                generator.IncreaseEnemiesViewAngle(GameSettings.Instanse.IncreaseInResidentViewAngle);
                generator.IncreaseEnemiesViewDistance(GameSettings.Instanse.IncreaseInResidentViewDistance);
            }),
            new VisibilityEvent(VisibilityEventType.IncreasedSpeed, storage.Speed, () =>
                generator.IncreaseEnemiesSpeed(GameSettings.Instanse.IncreaseInResidentSpeed)),
            new VisibilityEvent(VisibilityEventType.DoubleLocks, storage.Padlock, () =>
                GameSettings.Instanse.DoubleLocks = true),
            new VisibilityEvent(VisibilityEventType.NoReactionToNoise, storage.Signal, () =>
                GameSettings.Instanse.NoResidentsReactionOnIntentionalNoise = true),
            new VisibilityEvent(VisibilityEventType.ExtraResident, storage.Suitcase, () =>
                generator.CreateEnemy(1)),
            new VisibilityEvent(VisibilityEventType.IncreasedHearingRadius, storage.Ear, () =>
                GameSettings.Instanse.IncreasedHearingRadius = true)
        };
        currentEvents = new List<VisibilityEvent>(events);

        void AddPoliceman()
        {
            generator.CreatePoliceman(1);
            SoundManager.Instanse.Play(Sound.Police);
        }
        callPoliceEvent = new VisibilityEvent(VisibilityEventType.Police, storage.PoliceBadge, AddPoliceman);
        newPolicemanEvent = new VisibilityEvent(VisibilityEventType.NewPoliceman, storage.PoliceBadge, AddPoliceman);
    }

    private void CallPolice(int visibilityLevel)
    {
        if (policeCalled)
            ExecuteEvent(newPolicemanEvent, visibilityLevel);
        else
        {
            policeCalled = true;
            ExecuteEvent(callPoliceEvent, visibilityLevel);
        }
    }

    private void ExecuteEvent(VisibilityEvent ev, int visibilityLevel)
    {
        ev.Action?.Invoke();
        ShowMessage(ev.Icon, visibilityLevel, ev.Message);
    }

    private void ShowMessage(Sprite icon, int visibilityLevel, string message)
    {
        messageQueue.Add(new MainMessage(icon, $"{Translation.GetVisibilityLevelName()} {visibilityLevel}", 
            message, Sound.NewVisibilityLevel, titleMessageColor, backgroundMessageColor));
    }
}

public class VisibilityEvent
{
    public VisibilityEventType Type { get; }
    public string Message { get; }
    public Sprite Icon { get; }
    public Action Action { get; }
    public Sound Sound { get; }
    public bool HaveSound { get; } = false;

    public VisibilityEvent(VisibilityEventType type, Sprite icon, Action action)
    {
        Type = type;
        Message = Translation.Get(type);
        Icon = icon;
        Action = action;
    }

    public VisibilityEvent(VisibilityEventType type, Sprite icon, Action action, Sound sound)
        : this(type, icon, action)
    {
        HaveSound = true;
        Sound = sound;
    }
}
