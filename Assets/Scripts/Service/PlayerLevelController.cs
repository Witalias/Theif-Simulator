using System;
using YG;

public class PlayerLevelController
{
    public static event Action<int> NewLevelReached;
    public static event Action<string, float, bool> ShowQuickMessage;

    private XPBar _xpBar;
    private int _neededXP;
    private int _xpAmount;

    public int Level { get; private set; } = 1;

    public PlayerLevelController(XPBar xpBar, int neededXP)
    {
        _xpBar = xpBar;
        if (SaveLoad.HasLevelSave)
        {
            Level = YandexGame.savesData.Level;
            _xpAmount = YandexGame.savesData.CurrentXP;
            _neededXP = YandexGame.savesData.RequiredXP;
            NewLevelReached?.Invoke(Level);
        }
        else
        {
            _neededXP = neededXP;
        }
        _xpBar.SetLevel(Level);
        UpdateXPBar();
    }

    public void AddXP(int value)
    {
        _xpAmount += value;
        if (_xpAmount >= _neededXP)
        {
            _xpAmount -= _neededXP;
            NextLevel();
        }
        UpdateXPBar();
        SaveLoad.SaveLevel(Level, _xpAmount, _neededXP);
    }

    public void AddXPToNextLevel()
    {
        AddXP(_neededXP - _xpAmount);
    }

    private void NextLevel()
    {
        SoundManager.Instanse.Play(Sound.NewLevel);
        _xpBar.SetLevel(++Level);
        _xpBar.ActiveConfetti();
        _neededXP += GameData.Instanse.StepXPRequirement;
        ShowQuickMessage?.Invoke($"{Translation.GetNewLevelName()}!", 3.0f, false);
        NewLevelReached?.Invoke(Level);
        MetricaSender.PlayerLevel(Level);
    }

    private void UpdateXPBar()
    {
        _xpBar.SetProgress(_xpAmount, _neededXP);

        if (Level >= GameData.Instanse.MaxLevel)
            _xpBar.SetMaxLevelState();
    }
}
