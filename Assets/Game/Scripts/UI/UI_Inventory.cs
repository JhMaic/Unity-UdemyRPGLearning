using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using Observable = R3.Observable;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private Transform inventoryUI;
    [SerializeField] private Transform equipmentUI;


    private Equipment _equipment;

    private List<UI_ItemSlot> _equipmentSlotsUI;

    private Inventory _inventory;

    private List<UI_ItemSlot> _itemSlotsUI;

    private MouseHoldingData _mouseHoldingData;

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
        _equipment = GetComponent<Equipment>();
        _inventory.SlotChanged += OnSlotChanged;
        _itemSlotsUI = inventoryUI.GetComponentsInChildren<UI_ItemSlot>().ToList();
        _equipmentSlotsUI = equipmentUI.GetComponentsInChildren<UI_ItemSlot>().ToList();


        for (var i = 0; i < _itemSlotsUI.Count; i++)
        {
            _itemSlotsUI[i].MouseDown += OnItemSlotMouseDown;
            _itemSlotsUI[i].MouseUp += OnItemSlotMouseUp;
            _itemSlotsUI[i].SlotIndex = i;
        }

        for (var i = 0; i < _equipmentSlotsUI.Count; i++)
        {
            _equipmentSlotsUI[i].MouseDown += OnEquipmentSlotMouseDown;
            _equipmentSlotsUI[i].MouseUp += OnEquipmentSlotMouseUp;
            _equipmentSlotsUI[i].SlotIndex = i;
        }
    }

    private void OnDestroy()
    {
        _inventory.SlotChanged -= OnSlotChanged;

        foreach (var itemSlot in _itemSlotsUI)
        {
            itemSlot.MouseDown -= OnItemSlotMouseDown;
            itemSlot.MouseUp -= OnItemSlotMouseUp;
        }

        foreach (var equipmentSlot in _equipmentSlotsUI)
        {
            equipmentSlot.MouseDown -= OnEquipmentSlotMouseDown;
            equipmentSlot.MouseUp -= OnEquipmentSlotMouseUp;
        }
    }

    private void OnSlotChanged(int idx)
    {
        _itemSlotsUI[idx].UpdateUI(_inventory.ItemSlots[idx]);
    }

    private void OnItemSlotMouseDown(int idx)
    {
        if (!_mouseHoldingData.isHolding)
        {
            // set color when item is selected
            var ui_slot = _itemSlotsUI[idx].GetComponent<UI_ItemSlot>();

            if (ui_slot.item?.Item is null)
                return;

            var restoreOpacity = ui_slot.SetOpacity(0.5f);

            // the first time that the mouse clicked
            _mouseHoldingData.isHolding = true;
            _mouseHoldingData.itemSlotIndex = idx;
            var disposable = Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ =>
                    {
                        FinishMouseHolding();
                        restoreOpacity();
                    }
                );

            _mouseHoldingData.finishedCallback = () => { disposable.Dispose(); };
        }
        else
        {
            _inventory.SwapPosition(_mouseHoldingData.itemSlotIndex, idx);
            FinishMouseHolding();
        }
    }


    private void OnItemSlotMouseUp(int idx)
    {
    }

    public void OnEquipmentSlotMouseDown(int idx)
    {
        if (_mouseHoldingData.isHolding)
        {
            var holdingItem = _itemSlotsUI[_mouseHoldingData.itemSlotIndex].item;

            if (holdingItem.Item is ItemData_Equipment equipment)
            {
                _equipment.SetEquipment(equipment, out var oldOne);
                _inventory.ConsumeItem(_mouseHoldingData.itemSlotIndex);

                if (oldOne is not null)
                    _inventory.AutoFill(oldOne);

                var e_idx = equipment.equipmentType switch
                {
                    EquipmentType.Weapon => 0,
                    EquipmentType.Armor => 1,
                    EquipmentType.Amulet => 2,
                    _ => 3
                };
                _equipmentSlotsUI[e_idx].UpdateUI(holdingItem);
            }

            FinishMouseHolding();
        }
    }

    public void OnEquipmentSlotMouseUp(int idx)
    {
    }

    private void FinishMouseHolding()
    {
        // set default
        _mouseHoldingData.isHolding = false;

        _mouseHoldingData.finishedCallback();
        _mouseHoldingData.finishedCallback = null;
    }

    private struct MouseHoldingData
    {
        public bool isHolding;
        public int itemSlotIndex;

        /// <summary>
        ///     callback processing when mouse holding is finished
        /// </summary>
        public Action finishedCallback;
    }
}