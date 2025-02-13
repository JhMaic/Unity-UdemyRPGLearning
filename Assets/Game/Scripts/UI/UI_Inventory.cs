using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private Transform inventoryUI;
    private Inventory _inventory;
    private List<UI_ItemSlot> _itemSlotsUI;

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
        _inventory.SlotChanged += OnSlotChanged;
        _itemSlotsUI = inventoryUI.GetComponentsInChildren<UI_ItemSlot>().ToList();

        for (var i = 0; i < _itemSlotsUI.Count; i++)
        {
            _itemSlotsUI[i].MouseDown += OnSlotMouseDown;
            _itemSlotsUI[i].MouseUp += OnSlotMouseUp;
            _itemSlotsUI[i].SlotIndex = i;
        }
    }

    private void OnDestroy()
    {
        _inventory.SlotChanged -= OnSlotChanged;

        foreach (var itemSlot in _itemSlotsUI)
        {
            itemSlot.MouseDown -= OnSlotMouseDown;
            itemSlot.MouseUp -= OnSlotMouseUp;
        }
    }

    private void OnSlotChanged(int idx)
    {
        _itemSlotsUI[idx].UpdateUI(_inventory.ItemSlots[idx]);
    }

    private void OnSlotMouseDown(int idx)
    {
        Debug.Log(idx);
    }

    private void OnSlotMouseUp(int idx)
    {
    }
}