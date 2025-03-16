using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public static Equipment Instance;

    public List<EquipmentSlot> Slot { get; private set; }


    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        Slot = new List<EquipmentSlot>
        {
            new("equipment_weapon_1", EquipmentType.Weapon),
            new("equipment_armor_1", EquipmentType.Armor),
            new("equipment_amulet_1", EquipmentType.Amulet),
            new("equipment_flask_1", EquipmentType.Flask)
        };
    }
}