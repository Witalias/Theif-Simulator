using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    public static CameraChanger Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera _mainCamera;

    public void TemporarilySwitchCamera(CinemachineVirtualCamera toCamera, float delay)
    {
        toCamera.Priority = _mainCamera.Priority + 1;
        DOVirtual.DelayedCall(delay, () => toCamera.Priority = 0);
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
