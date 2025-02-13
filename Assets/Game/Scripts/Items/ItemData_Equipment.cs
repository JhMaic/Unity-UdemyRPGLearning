using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;
}