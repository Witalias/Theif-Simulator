using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [Serializable]
    private class TaskData
    {
        public TaskType Type;
        public Vector2 MinMaxRequirenment;
        public int RewardMoney;
        public Sprite Sprite;
    }

    public static event Action<int> PlayResourceAnimationMoney;

    [SerializeField] private TaskData[] _taskData;
    [SerializeField] private TaskPanel _taskPanel;

    private TaskType _currentTask;
    private ResourceType _requiredResource;
    private int _requiredCount;
    private int _currentCount;
    private int _reward;
    private Dictionary<TaskType, TaskData> _taskDataDict = new();
    private List<TaskType> _availableTasks = new()
    {
        TaskType.EarnMoney,
        TaskType.HackHouse,
        TaskType.SellItems,
        TaskType.BuyUpgrade,
        TaskType.SellCertainItems,
        TaskType.TheftCertainItems,
        TaskType.TheftItems
    };
    private List<ResourceType> _availableResources = new()
    {
        ResourceType.Bottle,
        ResourceType.Sneakers
    };

    public void StartRandomTask()
    {
        var task = _currentTask;
        while (task == _currentTask)
            task = _availableTasks[UnityEngine.Random.Range(0, _availableTasks.Count)];
        _currentTask = task;
        _requiredCount = Randomizator.GetRandomValue(_taskDataDict[task].MinMaxRequirenment);
        _currentCount = 0;
        _reward = _taskDataDict[task].RewardMoney;

        Sprite taskIcon;
        var isResourceTask = (task == TaskType.TheftCertainItems || task == TaskType.SellCertainItems);
        if (isResourceTask)
        {
            _requiredResource = _availableResources[UnityEngine.Random.Range(0, _availableResources.Count)];
            taskIcon = GameStorage.Instanse.GetResourceSprite(_requiredResource);
        }
        else
        {
            taskIcon = _taskDataDict[task].Sprite;
        }
        UpdateProgressBar();

        if (_taskPanel != null)
            _taskPanel.Show(taskIcon, GetTaskDescription(), _reward);
    }

    public void RemoveAvailableTask(TaskType task)
    {
        if (_availableTasks.Contains(task))
            _availableTasks.Remove(task);
    }

    public void AddAvailableResources(params ResourceType[] types)
    {
        _availableResources.AddRange(types);
    }

    public void ProcessTask(TaskType incomeTask, ResourceType incomeResource, int count)
    {
        if (incomeTask != _currentTask || incomeResource != _requiredResource)
            return;

        AddProgress(count);
    }

    public void ProcessTask(TaskType incomeTask, int count)
    {
        if (incomeTask != _currentTask)
            return;

        AddProgress(count);
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        foreach (var task in _taskData)
            _taskDataDict.Add(task.Type, task);
    }

    private void Start()
    {
        StartRandomTask();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            StartRandomTask();
    }

    private void AddProgress(int value)
    {
        _currentCount += value;
        UpdateProgressBar();

        if (_currentCount >= _requiredCount)
            Complete();
    }

    private void Complete()
    {
        Stats.Instanse.AddMoney(_reward, false);
        PlayResourceAnimationMoney?.Invoke(_reward);
        SoundManager.Instanse.Play(Sound.GetMoney);
        DOVirtual.DelayedCall(1.0f, StartRandomTask);
    }

    private string GetTaskDescription()
    {
        return GameSettings.Instanse.Language switch
        {
            Language.English => _currentTask switch
            {
                TaskType.SellItems => $"Sell {_requiredCount} items",
                TaskType.SellCertainItems => $"Sell {_requiredCount} {Translation.GetResourceName(_requiredResource, true).ToLower()}",
                TaskType.TheftItems => $"Theft {_requiredCount} items",
                TaskType.TheftCertainItems => $"Theft {_requiredCount} {Translation.GetResourceName(_requiredResource, true).ToLower()}",
                TaskType.EarnMoney => $"Earn {_requiredCount} money",
                TaskType.BuyUpgrade => $"Buy {_requiredCount} upgrade",
                TaskType.HackHouse => $"Hack {_requiredCount} buildings",
                _ => "",
            },
            Language.Russian => _currentTask switch
            {
                TaskType.SellItems => $"Продать {_requiredCount} предметов",
                TaskType.SellCertainItems => $"Продать {_requiredCount} {Translation.GetResourceName(_requiredResource, true).ToLower()}",
                TaskType.TheftItems => $"Украсть {_requiredCount} предметов",
                TaskType.TheftCertainItems => $"Украсть {_requiredCount} {Translation.GetResourceName(_requiredResource, true).ToLower()}",
                TaskType.EarnMoney => $"Заработать {_requiredCount} денег",
                TaskType.BuyUpgrade => $"Купить {_requiredCount} улучшение",
                TaskType.HackHouse => $"Взломать {_requiredCount} домов",
                _ => "",
            },
            _ => "",
        };
    }

    private void UpdateProgressBar()
    {
        if (_taskPanel == null)
            return;

        _taskPanel.SetBarValue(_currentCount, _requiredCount);
    }
}
