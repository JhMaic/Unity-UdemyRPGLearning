using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;

internal class InventoryTest : MonoBehaviour
{
    [SerializeField] private InventoryV2 inventory;

    [MethodButton]
    public void Consume(int idx, int count)
    {
        inventory.RemoveSlot(idx);
    }

    [MethodButton]
    public void Consume(string itemName, int count)
    {
        inventory.Consume(itemName, count);
    }
}