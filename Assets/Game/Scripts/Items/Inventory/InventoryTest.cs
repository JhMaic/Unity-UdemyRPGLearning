using CustomInspector;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    [SerializeField] [SelfFill(true)]
    private Inventory inventory;


    [Button(nameof(TestConsumeOne), true)]
    public int slotIdx;

    [Button(nameof(TestConsumeMany), true)]
    public int count;

    public void TestConsumeOne(int _slotIdx)
    {
        inventory.ConsumeItem(_slotIdx);
    }

    public void TestConsumeMany(int _count)
    {
        inventory.TryConsumeItems("Animal Skin", _count);
    }
}