using UnityEngine;
using System;
using System.Collections.Generic;

public class Skills : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color usualTitleColor;
    [SerializeField] private Color usualBackgroundColor;
    [SerializeField] private Color coolTitleColor;
    [SerializeField] private Color coolBackgroundColor;
    [SerializeField] private Color epicTitleColor;
    [SerializeField] private Color epicBackgroundColor;

    private MovementController movementController;

    private Dictionary<SkillCoolness, (Color title, Color background)> messageColors;
    private List<Skill> skills;

    private void Awake()
    {
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

        skills = new List<Skill>
        {
            new Skill(value => movementController.AddSpeed(value),
            new[] { (10f, SkillCoolness.Usual), (20f, SkillCoolness.Usual), (40f, SkillCoolness.Cool), (60f, SkillCoolness.Cool), (100f, SkillCoolness.Epic) },
            true, SkillType.FastFeet),

            new Skill(value => GameSettings.Instanse.AddHearingRadiusAfterOpeningDoor(value),
            new[] { (-10f, SkillCoolness.Usual), (-20f, SkillCoolness.Usual), (-40f, SkillCoolness.Cool), (-60f, SkillCoolness.Cool), (-100f, SkillCoolness.Epic) },
            true, SkillType.NotCheackingDoors)
        };
    }
}

public class Skill
{
    public bool AddPercents { get; } = false;
    public bool MaxLevel { get; private set; } = false;
    public Action<float> Action { get; }
    public (float value, SkillCoolness coolness)[] LevelValues { get; }
    public string Title { get; }
    public string Description { get; }

    private int currentLevel = 0;

    public Skill(Action<float> action, (float, SkillCoolness)[] levelValues, bool addPercents, SkillType type)
    {
        Action = action;
        LevelValues = levelValues;
        AddPercents = addPercents;
        Title = Translation.GetTitle(type);
        Description = Translation.GetDescription(type);

        if (levelValues == null || levelValues.Length == 0)
            MaxLevel = true;
    }

    public void Use()
    {
        if (MaxLevel)
            return;

        Action?.Invoke(LevelValues[currentLevel++].value);

        if (currentLevel == LevelValues.Length)
            MaxLevel = true;
    }

}