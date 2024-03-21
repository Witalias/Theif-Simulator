using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OpenClosePopup : MonoBehaviour
{
    public static event Action<bool> Opened;
    public static event Action<bool> OpenedLate;

    [SerializeField] private float _animationDuration = 0.25f;
    [SerializeField] private GameObject _anticlick;
    [SerializeField] private Transform _content;
    [SerializeField] private Button _closeButton;

    private Tween _tween;
    private float _defaultScale;
    private bool _opened;

    private void Awake()
    {
        _defaultScale = _content.localScale.x;
        _content.localScale = Vector3.zero;
        _content.gameObject.SetActive(true);
        _closeButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        HumanAI.PlayerIsNoticed += Close;
    }

    private void OnDisable()
    {
        HumanAI.PlayerIsNoticed -= Close;
    }

    public void Open()
    {
        if (_opened)
            return;

        _opened = true;
        _anticlick.SetActive(true);
        _tween.Kill();
        _tween = DOTween.Sequence()
            .Append(_content.DOScale(_defaultScale + 0.1f, _animationDuration))
            .Append(_content.DOScale(_defaultScale, 0.2f))
            .Play();
        //_content.DOScale(_defaultScale, _animationDuration);
        Opened?.Invoke(true);
        OpenedLate?.Invoke(true);
    }

    private void Close()
    {
        if (!_opened)
            return;

        AudioManager.Instanse.Play(AudioType.Tap);
        _opened = false;
        _anticlick.SetActive(false);
        _tween.Kill();
        _tween = DOTween.Sequence()
            .Append(_content.DOScale(_defaultScale + 0.1f, 0.2f))
            .Append(_content.DOScale(Vector3.zero, _animationDuration))
            .Play();
        //_content.DOScale(Vector3.zero, _animationDuration);
        Opened?.Invoke(false);
        OpenedLate?.Invoke(false);
    }
}
