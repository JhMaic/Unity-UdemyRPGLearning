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
    }

    private void OnDestroy()
    {
        _inventory.SlotChanged -= OnSlotChanged;
    }

    private void OnSlotChanged(int idx)
    {
        _itemSlotsUI[idx].UpdateUI(_inventory.ItemSlots[idx]);
    }
}