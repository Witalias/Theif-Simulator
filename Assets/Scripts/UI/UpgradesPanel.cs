using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesPanel : MonoBehaviour
{
    public static event Action<bool> Opened;

    [SerializeField] private float _animationDuration;
    [SerializeField] private GameObject _anticlick;
    [SerializeField] private Transform _content;
    [SerializeField] private Button _closeButton;

    private bool _opened;

    private void Awake()
    {
        _content.localScale = Vector3.zero;
        _content.gameObject.SetActive(true);
        _closeButton.onClick.AddListener(Close);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            Open();
    }

    private void Open()
    {
        if (_opened)
            return;

        _opened = true;
        _anticlick.SetActive(true);
        _content.DOScale(Vector3.one, _animationDuration);
        Opened?.Invoke(true);
    }

    private void Close()
    {
        _opened = false;
        _anticlick.SetActive(false);
        _content.DOScale(Vector3.zero, _animationDuration);
        Opened?.Invoke(false);
    }
}
