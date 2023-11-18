using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static event Action<bool> HoldButtonActived;

    [SerializeField] private Color _negativeColor;
    [SerializeField] private Color _positiveColor;
    [SerializeField] private GameObject _content;
    [SerializeField] private Image _filledImage;
    [SerializeField] private Animation _animation;

    private Image _button;
    private Action _actionDone;
    private Action _actionAbort;
    private bool _mouseDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        _mouseDown = true;
        transform.DOScale(0.9f, 0.25f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _mouseDown = false;
        transform.DOScale(1.0f, 0.25f);
    }

    private void Awake()
    {
        _button = GetComponent<Image>();
    }

    private void OnEnable()
    {
        Lootable.ShowHoldButton += Show;
        MovementController.PlayerCaught += Abort;
        EnemyAI.PlayerIsNoticed += Abort;
        UpgradesPanel.Opened += (open) => Abort();
    }

    private void OnDisable()
    {
        Lootable.ShowHoldButton -= Show;
        MovementController.PlayerCaught -= Abort;
        EnemyAI.PlayerIsNoticed -= Abort;
        UpgradesPanel.Opened -= (open) => Abort();
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

        _filledImage.fillAmount += Time.deltaTime * Stats.Instanse.FillSpeedForHoldButton;
        _filledImage.color = Color.Lerp(_negativeColor, _positiveColor, _filledImage.fillAmount);

        if (_filledImage.fillAmount >= 1.0f)
        {
            _actionDone?.Invoke();
            SetActive(false);
        }
    }

    private void Abort()
    {
        if (!_content.activeSelf)
            return;

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
            _animation.Play();
        }
        else
        {
            _actionAbort = null;
            _actionDone = null;
        }
        HoldButtonActived?.Invoke(value);
    }
}
