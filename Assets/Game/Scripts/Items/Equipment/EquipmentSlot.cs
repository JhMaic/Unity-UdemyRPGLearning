using System;

public class EquipmentSlot : Slot
{
    public readonly EquipmentType equipmentType;

    public EquipmentSlot(string slotId, EquipmentType type) : base(slotId)
    {
        equipmentType = type;
    }

    public override void PutItem(SlotItem item)
    {
        if (item == null)
        {
            RemoveItem();
            return;
        }

        if (item?.Item is ItemData_Equipment equipment && equipment.equipmentType.Equals(equipmentType))
            base.PutItem(item);

        else
            throw new Exception("Item type Error");
    }
}