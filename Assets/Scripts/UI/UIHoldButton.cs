using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static event Action<bool> HoldButtonActived;

    [SerializeField] private float _fillSpeed;
    [SerializeField] private GameObject _content;
    [SerializeField] private Image _filledImage;

    private Image _button;
    private Action _actionDone;
    private Action _actionAbort;
    private bool _mouseDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        _mouseDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _mouseDown = false;
    }

    private void Awake()
    {
        _button = GetComponent<Image>();
    }

    private void OnEnable()
    {
        Lootable.ShowHoldButton += Show;
    }

    private void OnDisable()
    {
        Lootable.ShowHoldButton -= Show;
    }

    private void Update()
    {
        if (_mouseDown)
            OnHold();
    }

    private void OnHold()
    {
        if (!_content.activeSelf)
            return;

        _filledImage.fillAmount += Time.deltaTime * _fillSpeed;

        if (_filledImage.fillAmount >= 1.0f)
        {
            _actionDone?.Invoke();
            SetActive(false);
        }
    }

    private void Abort()
    {
        _actionAbort?.Invoke();
        SetActive(false);
    }

    private void Show(Action actionDone, Action actionAbort)
    {
        _actionDone = actionDone;
        _actionAbort = actionAbort;
        SetActive(true);
    }

    private void SetActive(bool value)
    {
        _content.SetActive(value);
        _button.enabled = value;

        if (value)
        {
            _filledImage.fillAmount = 0.0f;
            MovementController.MovingStarted += Abort;
        }
        else
        {
            _actionAbort = null;
            _actionDone = null;
            MovementController.MovingStarted -= Abort;
        }
        HoldButtonActived?.Invoke(value);
    }
}
