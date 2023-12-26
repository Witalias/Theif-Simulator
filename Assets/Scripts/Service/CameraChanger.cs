using Cinemachine;
using DG.Tweening;
using System;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    public static CameraChanger Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _indoorCamera;

    private CinemachineVirtualCamera _currentCamera;

    public void TemporarilySwitchCamera(CinemachineVirtualCamera toCamera, float delay, Action onComlete = null)
    {
        toCamera.Priority = _currentCamera.Priority + 1;
        DOVirtual.DelayedCall(delay, () => toCamera.Priority = 0).OnComplete(() => onComlete?.Invoke());
    }

    public void SwitchToMainCamera()
    {
        _mainCamera.Priority = 10;
        _currentCamera.Priority = 0;
        _currentCamera = _mainCamera;
    }

    public void SwitchToIndoorCamera()
    {
        _indoorCamera.Priority = 10;
        _currentCamera.Priority = 0;
        _currentCamera = _indoorCamera;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _currentCamera = _mainCamera;
    }
}
