using UnityEngine;
using UnityEngine.UI;
using YG;

public class TutorialStealthPopup : MonoBehaviour
{
    [SerializeField] private Image _phone;
    [SerializeField] private Sprite _wasd;

    private void Start()
    {
        if (YandexGame.EnvironmentData.isDesktop)
            _phone.sprite = _wasd;
    }
}
