using UnityEngine;

public class UI_ItemManager : MonoBehaviour
{
    [SerializeField] private Transform inventoryUI;
    [SerializeField] private Transform equipmentUI;

    private Equipment _equipment;
    private InventoryV2 _inventory;

    private void Start()
    {
        _inventory = GetComponent<InventoryV2>();
        _equipment = GetComponent<Equipment>();

        var slotUIs = inventoryUI.GetComponentsInChildren<UI_ItemSlot>();
        var equipmentSlotUIs = equipmentUI.GetComponentsInChildren<UI_ItemSlot>();

        for (var i = 0; i < slotUIs.Length; i++)
            slotUIs[i].Init(_inventory.Slots[i]);

        for (var i = 0; i < equipmentSlotUIs.Length; i++)
            equipmentSlotUIs[i].Init(_equipment.Slot[i]);
    }
}