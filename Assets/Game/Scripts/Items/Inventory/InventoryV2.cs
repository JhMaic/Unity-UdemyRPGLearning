using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryV2 : MonoBehaviour
{
    public static InventoryV2 Instance;

    [SerializeField]
    private int maxSlotCount = 64;

    public List<Slot> Slots { get; private set; }
    private IEnumerable<Slot> SlotsNotEmpty => Slots.Where(s => s.Stat != Slot.Status.Empty);


    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        Slots = Enumerable.Range(0, 64)
            .Select(idx => new Slot($"inventory_slot_{idx}"))
            .ToList();
    }

    /// <summary>
    ///     auto fill inventory
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="count"></param>
    public void AutoFill(ItemData itemData, int count = 1)
    {
        // find slots with the same item but is not full
        var slots = Slots.Where(
            slot => slot.Stat == Slot.Status.Some &&
                    slot.Item.Name.Equals(itemData.itemName));

        var number = count;
        foreach (var slot in slots)
        {
            if (number == 0)
                break;

            var added = slot.Item.Add(number);
            number -= added;
        }

        // if still has few remaining, create new slot data
        if (number != 0)
        {
            var slotItems = CreateSlots(itemData, number);
            AddToEmptySlot(slotItems);
        }
    }

    public void AddToEmptySlot(List<SlotItem> slotItems)
    {
        slotItems.ForEach(AddToEmptySlot);
    }

    public void AddToEmptySlot(SlotItem slotItem)
    {
        var slot = FindEmptySlot();
        slot.PutItem(slotItem);
    }

    public bool Consume(string itemName, int count)
    {
        var total = GetTotal(itemName);

        if (count > total)
            return false;

        var itemSlots = SlotsNotEmpty
            .Where(s => s.Item.Name.Equals(itemName));

        var tmpNumber = count;
        foreach (var itemSlot in itemSlots)
        {
            if (tmpNumber == 0)
                break;

            var removed = itemSlot.Item.Remove(tmpNumber);
            tmpNumber -= removed;
        }

        return true;
    }

    public void RemoveSlot(int idx)
    {
        Slots[idx].RemoveItem();
    }


    private List<SlotItem> CreateSlots(ItemData itemData, int count)
    {
        List<SlotItem> data = new();

        var tmpCount = count;
        while (true)
        {
            var item = SlotItem.Create(itemData, tmpCount, out var restCount);
            data.Add(item);

            if (restCount == 0)
                break;

            tmpCount = restCount;
        }

        return data;
    }


    private Slot FindEmptySlot()
    {
        return Slots.Find(slot => slot.Stat == Slot.Status.Empty);
    }

    private int GetTotal(string itemName)
    {
        return SlotsNotEmpty.Where(slot => slot.Item.Name.Equals(itemName))
            .Select(s => s.Item.Count)
            .Sum();
    }
}