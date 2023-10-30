using UnityEngine;
using System;
using System.Collections.Generic;

public class Skills : MonoBehaviour
{
    public static Skills Instanse { get; private set; }

    [Header("Colors")]
    [SerializeField] private Color usualTitleColor;
    [SerializeField] private Color usualBackgroundColor;
    [SerializeField] private Color coolTitleColor;
    [SerializeField] private Color coolBackgroundColor;
    [SerializeField] private Color epicTitleColor;
    [SerializeField] private Color epicBackgroundColor;
    [SerializeField] private string valueColorCode = "#460e8a";

    private MovementController movementController;
    private MessageQueue messageQueue;
    private GameStorage storage;

    private Dictionary<SkillCoolness, (Color title, Color background)> messageColors;
    private List<Skill> skills;

    public void UseRandom()
    {
        var randomSkill = skills[UnityEngine.Random.Range(0, skills.Count)];

        var level = "";
        var stringValue = "";
        var sign = "";
        var percent = "";

        if (randomSkill.LevelValues != null && randomSkill.LevelValues[randomSkill.Level].value != 0)
        {
            level = (randomSkill.Level + 1).ToString();
            var value = randomSkill.LevelValues[randomSkill.Level].value;
            sign = value > 0 ? "+" : "—";
            percent = randomSkill.AddPercent ? "%" : "";
            stringValue = ((int)Mathf.Abs(value)).ToString();
        }
        var title = $"{randomSkill.Title} {level}";
        var description = $"{randomSkill.Description} <b><color={valueColorCode}>{sign}{stringValue}{percent}</color></b>";

        if (randomSkill.LevelValues == null)
            messageQueue.Add(new MainMessage(randomSkill.Icon, title, description, Sound.NewSkill));
        else
        {
            var coolness = randomSkill.LevelValues[randomSkill.Level].coolness;
            messageQueue.Add(new MainMessage(randomSkill.Icon, title, description, Sound.NewSkill, messageColors[coolness].title, messageColors[coolness].background));
        }

        randomSkill.Use();
        if (randomSkill.MaxLevel)
            skills.Remove(randomSkill);

        AddNewSkills(randomSkill.Type);
    }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        messageColors = new Dictionary<SkillCoolness, (Color title, Color background)>
        {
            [SkillCoolness.Usual] = (usualTitleColor, usualBackgroundColor),
            [SkillCoolness.Cool] = (coolTitleColor, coolBackgroundColor),
            [SkillCoolness.Epic] = (epicTitleColor, epicBackgroundColor)
        };
    }

    private void Start()
    {
        movementController = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
        messageQueue = GameObject.FindGameObjectWithTag(Tags.MessageQueue.ToString()).GetComponent<MessageQueue>();
        storage = GameStorage.Instanse;

        skills = new List<Skill>
        {
            new Skill(value => Stats.Instanse.SetIncreasedPlayerSpeed(value),
            new[] { (5f, SkillCoolness.Usual), (10f, SkillCoolness.Usual), (15f, SkillCoolness.Cool), (25f, SkillCoolness.Cool), (40f, SkillCoolness.Epic) },
            true, SkillType.FastFeet, storage.Boot),

            new Skill(value => Stats.Instanse.SetIncreasedDoorNoise(value),
            new[] { (-10f, SkillCoolness.Usual), (-20f, SkillCoolness.Usual), (-30f, SkillCoolness.Cool), (-50f, SkillCoolness.Cool), (-70f, SkillCoolness.Epic) },
            true, SkillType.NotCheackingDoors, storage.Door),

            new Skill(value => Stats.Instanse.GetEquipmentStats(EquipmentType.Arms).IncreasedHackingTimeInPercents = value,
            new[] { (-10f, SkillCoolness.Usual), (-20f, SkillCoolness.Usual), (-30f, SkillCoolness.Cool), (-50f, SkillCoolness.Cool), (-70f, SkillCoolness.Epic) },
            true, SkillType.PracticeOfManualHacking, storage.ArmsTime),

            new Skill(value => Stats.Instanse.GetEquipmentStats(EquipmentType.Arms).IncreasedNoiseInPercents = value,
            new[] { (-10f, SkillCoolness.Usual), (-20f, SkillCoolness.Usual), (-30f, SkillCoolness.Cool), (-40f, SkillCoolness.Cool), (-50f, SkillCoolness.Epic) },
            true, SkillType.AgilityOfManualHacking, storage.ArmsEar),

            new Skill(value => Stats.Instanse.GetEquipmentStats(EquipmentType.MasterKey).IncreasedHackingTimeInPercents = value,
            new[] { (-10f, SkillCoolness.Usual), (-20f, SkillCoolness.Usual), (-30f, SkillCoolness.Cool), (-50f, SkillCoolness.Cool), (-70f, SkillCoolness.Epic) },
            true, SkillType.PracticeOfHackingWithAMasterKey, storage.MasterKeyTime),

            new Skill(value => Stats.Instanse.GetEquipmentStats(EquipmentType.TierIron).IncreasedHackingTimeInPercents = value,
            new[] { (-10f, SkillCoolness.Usual), (-20f, SkillCoolness.Usual), (-30f, SkillCoolness.Cool), (-50f, SkillCoolness.Cool), (-70f, SkillCoolness.Epic) },
            true, SkillType.PracticeOfHackingWithACrowbar, storage.TierIronTime),

            new Skill(value => Stats.Instanse.GetEquipmentStats(EquipmentType.TierIron).IncreasedNoiseInPercents = value,
            new[] { (-10f, SkillCoolness.Usual), (-20f, SkillCoolness.Usual), (-30f, SkillCoolness.Cool), (-40f, SkillCoolness.Cool), (-50f, SkillCoolness.Epic) },
            true, SkillType.AgilityOfHackingWithACrowbar, storage.TierIronEar),

            new Skill(value => Stats.Instanse.GetEquipmentStats(EquipmentType.Gadget).IncreasedHackingTimeInPercents = value,
            new[] { (-10f, SkillCoolness.Usual), (-20f, SkillCoolness.Usual), (-30f, SkillCoolness.Cool), (-50f, SkillCoolness.Cool), (-70f, SkillCoolness.Epic) },
            true, SkillType.PracticeOfHackingWithAGadget, storage.GadgetTime),

            new Skill(value => Stats.Instanse.IncreasedHackingTime = value,
            new[] { (-5f, SkillCoolness.Usual), (-10f, SkillCoolness.Usual), (-15f, SkillCoolness.Cool), (-20f, SkillCoolness.Cool), (-25f, SkillCoolness.Epic) },
            true, SkillType.PracticeOfHacking, storage.EquipmentTime),

            new Skill(value => Stats.Instanse.IncreasedHackingNoiseInPercents = value,
            new[] { (-5f, SkillCoolness.Usual), (-10f, SkillCoolness.Usual), (-15f, SkillCoolness.Cool), (-20f, SkillCoolness.Cool), (-25f, SkillCoolness.Epic) },
            true, SkillType.AgilityOfHacking, storage.EquipmentEar),

            new Skill(value => Stats.Instanse.CanIntentionallyNoise = true, new[] { (0f, SkillCoolness.Cool) }, false, SkillType.SlyFox, storage.Signal),

            new Skill(value => Stats.Instanse.SetExtraResourceNumber(ResourceType.Food, value),
            new[] { (1f, SkillCoolness.Usual), (2f, SkillCoolness.Usual), (3f, SkillCoolness.Cool), (4f, SkillCoolness.Cool), (5f, SkillCoolness.Epic) },
            true, SkillType.MoreFood, storage.ApplePlus),

            new Skill(value => Stats.Instanse.SetExtraResourceNumber(ResourceType.Water, value),
            new[] { (1f, SkillCoolness.Usual), (2f, SkillCoolness.Usual), (3f, SkillCoolness.Cool), (4f, SkillCoolness.Cool), (5f, SkillCoolness.Epic) },
            true, SkillType.MoreWater, storage.WaterPlus),

            new Skill(value => Stats.Instanse.SetExtraResourceNumber(ResourceType.Money, value),
            new[] { (10f, SkillCoolness.Usual), (20f, SkillCoolness.Usual), (40f, SkillCoolness.Cool), (60f, SkillCoolness.Cool), (100f, SkillCoolness.Epic) },
            false, SkillType.MoreMoney, storage.MoneyPlus),

            new Skill(value => Stats.Instanse.SetExtraResourceNumber(ResourceType.Fuel, value),
            new[] { (1f, SkillCoolness.Usual), (2f, SkillCoolness.Usual), (3f, SkillCoolness.Cool), (4f, SkillCoolness.Cool), (5f, SkillCoolness.Epic) },
            false, SkillType.MoreFuel, storage.FuelPlus),

            new Skill(value => Stats.Instanse.IncreasedVisibilityScaleInPercents = value,
            new[] { (-10f, SkillCoolness.Usual), (-20f, SkillCoolness.Usual), (-30f, SkillCoolness.Cool), (-40f, SkillCoolness.Cool), (-50f, SkillCoolness.Epic) },
            true, SkillType.LessVisibilityScale, storage.NoEye),
        };
    }

    private void AddNewSkills(SkillType previousSkill)
    {
        switch (previousSkill)
        {
            case SkillType.SlyFox: skills.Add(new Skill(value => Stats.Instanse.VisibilityFromIntentionalNoise = false,
                new[] { (0f, SkillCoolness.Epic) }, false, SkillType.NoVisibilityFromIntentionalNoise, storage.NoEye));
                break;
        }
    }
}

public class Skill
{
    public bool AddPercent { get; } = false;
    public bool MaxLevel { get; private set; } = false;
    public Action<float> Action { get; }
    public (float value, SkillCoolness coolness)[] LevelValues { get; }
    public string Title { get; }
    public string Description { get; }
    public int Level { get; private set; } = 0;
    public Sprite Icon { get; }
    public SkillType Type { get; }

    public Skill(Action<float> action, (float, SkillCoolness)[] levelValues, bool addPercents, SkillType type, Sprite icon)
    {
        Action = action;
        LevelValues = levelValues;
        AddPercent = addPercents;
        Title = Translation.GetTitle(type);
        Description = Translation.GetDescription(type);
        Icon = icon;
        Type = type;

        if (levelValues == null || levelValues.Length == 0)
            MaxLevel = true;
    }

    public void Use()
    {
        if (MaxLevel)
        {
            Action?.Invoke(0f);
            return;
        }

        Action?.Invoke(LevelValues[Level++].value);

        if (Level == LevelValues.Length)
            MaxLevel = true;
    }

}