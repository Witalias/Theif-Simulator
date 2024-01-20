using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

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
    public static event Action<TaskType> TaskCompleted;

    [SerializeField] private TaskData[] _taskData;
    [SerializeField] private TaskPanel _taskPanel;

    private TaskType _currentTask;
    private ResourceType _requiredResource;
    private int _requiredCount;
    private int _currentCount;
    private int _reward;
    private readonly Dictionary<TaskType, TaskData> _taskDataDict = new();
    private readonly List<TaskType> _availableTasks = new()
    {
        TaskType.EarnMoney,
        TaskType.HackHouse,
        TaskType.SellItems,
        TaskType.BuyUpgrade,
        TaskType.SellCertainItems,
        TaskType.TheftCertainItems,
        TaskType.TheftItems
    };
    private readonly List<ResourceType> _availableResources = new()
    {
        ResourceType.Bottle,
        ResourceType.Sneakers
    };

    public void StartTask(TaskType type, int requiredCount, int reward = -1, bool resetProgress = true)
    {
        var randomRequiredResource = _availableResources[UnityEngine.Random.Range(0, _availableResources.Count)];
        StartTask(type, requiredCount, randomRequiredResource, reward, resetProgress);
    }

    public void StartTask(TaskType type, int requiredCount, ResourceType requiredResource, int reward = -1, bool resetProgress = true)
    {
        _currentTask = type;
        _requiredCount = requiredCount;
        _reward = Mathf.Clamp(reward, 0, int.MaxValue);
        if (resetProgress)
            _currentCount = 0;

        Sprite taskIcon;
        var isResourceTask = (type == TaskType.TheftCertainItems || type == TaskType.SellCertainItems);
        if (isResourceTask)
        {
            _requiredResource = requiredResource;
            taskIcon = GameStorage.Instanse.GetResourceSprite(requiredResource);
        }
        else
        {
            taskIcon = _taskDataDict[type].Sprite;
        }
        UpdateProgressBar();

        if (_taskPanel != null)
            _taskPanel.Show(taskIcon, GetTaskDescription(), reward);

        Save();
    }

    public void StartRandomTask()
    {
        var task = _currentTask;
        while (task == _currentTask)
            task = _availableTasks[UnityEngine.Random.Range(0, _availableTasks.Count)];
        var requiredCount = Randomizator.GetRandomValue(_taskDataDict[task].MinMaxRequirenment);
        var reward = _taskDataDict[task].RewardMoney;
        StartTask(task, requiredCount, reward);
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
        Load();
    }

    private void Update()
    {
        if (GameSettings.Instanse.DebugMode && Input.GetKeyDown(KeyCode.F1))
            StartRandomTask();
    }

    private void AddProgress(int value)
    {
        _currentCount += value;
        UpdateProgressBar();

        if (_currentCount >= _requiredCount)
            Complete();
        else
            Save();
    }

    private void Complete()
    {
        if (_reward > 0)
        {
            Stats.Instanse.AddMoney(_reward, false);
            PlayResourceAnimationMoney?.Invoke(_reward);
            SoundManager.Instanse.Play(Sound.GetMoney);
        }
        SoundManager.Instanse.Play(Sound.TaskCompleted);
        TaskCompleted?.Invoke(_currentTask);
        _taskPanel.ActiveConfetti();
        if (YandexGame.savesData.TutorialDone)
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
                TaskType.TutorialCrackDoors => $"Crack the door",
                TaskType.TutorialRobHouse => $"Rob the house",
                TaskType.TutorialSellItems => $"Sell items",
                TaskType.TutorialBuyUpgrade => $"Buy upgrade",
                TaskType.TutorialBuyZone => $"Buy zone",
                _ => "",
            },
            Language.Russian => _currentTask switch
            {
                TaskType.SellItems => $"Продай {_requiredCount} предметов",
                TaskType.SellCertainItems => $"Продай {_requiredCount} {Translation.GetResourceName(_requiredResource, true).ToLower()}",
                TaskType.TheftItems => $"Укради {_requiredCount} предметов",
                TaskType.TheftCertainItems => $"Укради {_requiredCount} {Translation.GetResourceName(_requiredResource, true).ToLower()}",
                TaskType.EarnMoney => $"Заработай {_requiredCount} денег",
                TaskType.BuyUpgrade => $"Купи {_requiredCount} улучшение",
                TaskType.HackHouse => $"Взломай {_requiredCount} дом",
                TaskType.TutorialCrackDoors => $"Взломай дверь",
                TaskType.TutorialRobHouse => $"Ограбь дом",
                TaskType.TutorialSellItems => $"Продай предметы",
                TaskType.TutorialBuyUpgrade => $"Купи улучшение",
                TaskType.TutorialBuyZone => $"Купи область",
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

    private void Save()
    {
        if (!YandexGame.savesData.TutorialDone)
            return;

        SaveLoad.SaveTask(_currentTask, _requiredResource, _currentCount, _requiredCount, _reward);
    }

    private void Load()
    {
        if (!YandexGame.savesData.TutorialDone)
            return;

        if (SaveLoad.HasTaskSave)
        {
            var saves = YandexGame.savesData;
            var task = Enum.Parse<TaskType>(saves.TaskType);
            var requiredResource = Enum.Parse<ResourceType>(saves.RequiredResource);
            _currentCount = saves.CurrentTaskProgress;
            StartTask(task, saves.TaskRequirement, requiredResource, saves.TaskReward, false);
        }
        else
        {
            StartRandomTask();
        }    
    }
}
