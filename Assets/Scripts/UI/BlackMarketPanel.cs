using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OpenClosePopup))]
public class BlackMarketPanel : MonoBehaviour
{
    [SerializeField] private GameObject _saleSlotPrefab;
    [SerializeField] private Transform _saleSlotsParent;

    private OpenClosePopup _openClosePopup;
    private readonly List<SaleSlot> _saleSlots = new();

    private void Awake()
    {
        _openClosePopup = GetComponent<OpenClosePopup>();
    }

    private void Start()
    {
        CreateSlots();
    }

    private void OnEnable()
    {
        BlackMarketArea.PlayerStayed += Open;
    }

    private void OnDisable()
    {
        BlackMarketArea.PlayerStayed -= Open;
    }

    private void CreateSlots()
    {
        foreach (var resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            var slot = Instantiate(_saleSlotPrefab, _saleSlotsParent).GetComponent<SaleSlot>();
            slot.Initialize((ResourceType)resourceType);
            _saleSlots.Add(slot);
        }
    }

    private void Open()
    {
        _openClosePopup.Open();
        foreach (var slot in _saleSlots)
            slot.Refresh();
    }
}
