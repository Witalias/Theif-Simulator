using UnityEngine;
using YG;

public class TutorialControls : MonoBehaviour
{
    [SerializeField] private GameObject _mobileControls;
    [SerializeField] private GameObject _desktopControls;

    private void Awake()
    {
        var isDesktop = YandexGame.EnvironmentData.isDesktop;
        _desktopControls.SetActive(isDesktop);
        _mobileControls.SetActive(!isDesktop);
    }
}
