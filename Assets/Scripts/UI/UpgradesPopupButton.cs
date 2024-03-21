using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesPopupButton : MonoBehaviour
{
    public static event Action Clicked;

    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        AudioManager.Instanse.Play(AudioType.Tap);
        Clicked?.Invoke();
    }
}
