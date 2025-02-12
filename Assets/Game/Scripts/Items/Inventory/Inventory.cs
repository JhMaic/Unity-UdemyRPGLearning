using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public List<int> EmptySlotIdxes { get; private set; }
    public List<ItemSlot> ItemSlots { get; private set; }

    /// <summary>
    ///     key: key of the item
    ///     value: slot index of the item
    /// </summary>
    public Dictionary<string, List<int>> SlotIdxes { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        ItemSlots = new List<ItemSlot>();
        SlotIdxes = new Dictionary<string, List<int>>();
        EmptySlotIdxes = new List<int>();
    }

    /// <summary>
    ///     create/remove/update
    ///     slot_idx
    /// </summary>
    public event Action<int> SlotChanged;


    private bool TryGetEmptySlotIdx(out int idx)
    {
        idx = 0;
        if (EmptySlotIdxes.Count == 0)
            return false;

        idx = EmptySlotIdxes.Min();
        return true;
    }

    private void AddItem(ItemData item, int count = 1)
    {
        if (!SlotIdxes.ContainsKey(item.itemName))
        {
            var slots = ItemSlot.CreateMany(item, count);
            AddToSlots(slots);
        }
    }

    public void AutoFill(ItemData item, int count = 1)
    {
        // if item is already has slots in inventory, check if it's fulfilled
        if (SlotIdxes.TryGetValue(item.itemName, out var idxes))
        {
            var restCount = count;


            foreach (var idx in idxes)
            {
                if (ItemSlots[idx].IsFull)
                    continue;

                restCount = ItemSlots[idx].FillUp(restCount);

                if (restCount == 0)
                    break;
            }


            if (restCount != 0)
            {
                var slots = ItemSlot.CreateMany(item, restCount);
                AddToSlots(slots);
            }
        }

        // if not, add to new slots
        else
        {
            var slots = ItemSlot.CreateMany(item, count);
            AddToSlots(slots);
        }
    }


    private void UpdateSlotIdxes(string keyName, int idx)
    {
        if (SlotIdxes.TryGetValue(keyName, out var value))
        {
            value.Add(idx);
            value.Sort();
            return;
        }

        SlotIdxes.Add(keyName, new List<int> { idx });
    }


    private void AddToSlots(List<ItemSlot> itemSlots)
    {
        itemSlots.ForEach(AddToSlots);
    }

    public void ConsumeItem(int slotIdx)
    {
        var slot = ItemSlots[slotIdx];
        if (slot is null)
            return;

        var restCount = slot.RemoveOne();

        if (restCount == 0)
            RemoveSlot(slotIdx);
    }

    public bool TryConsumeItems(string keyName, int count)
    {
        var hasItem = SlotIdxes.TryGetValue(keyName, out var idxes);
        if (!hasItem)
            return false;

        var totalCount = idxes.Select(idx => ItemSlots[idx].Count).Sum();
        if (totalCount < count)
            return false;

        var restCount = count;
        foreach (var idx in idxes.ToList())
        {
            var slot = ItemSlots[idx];
            if (restCount >= slot.Count)
            {
                restCount -= slot.Count;
                RemoveSlot(idx);
            }
            else
            {
                slot.TryRemoveSome(restCount);
                restCount = 0;
            }

            if (restCount == 0)
                break;
        }

        return true;
    }


    private void AddToSlots(ItemSlot itemSlot)
    {
        int idx;
        if (TryGetEmptySlotIdx(out var emptySlotIdx))
        {
            idx = emptySlotIdx;
            ItemSlots[idx] = itemSlot;
            EmptySlotIdxes.Remove(idx);
            UpdateSlotIdxes(itemSlot.Item.itemName, idx);
        }
        else
        {
            ItemSlots.Add(itemSlot);
            idx = ItemSlots.Count - 1;
            UpdateSlotIdxes(itemSlot.Item.itemName, idx);
        }

        SlotChanged?.Invoke(idx);
        itemSlot.CountChanged += HandleItemCountChanged(idx);
    }

    private void RemoveSlot(int slotIdx)
    {
        if (ItemSlots[slotIdx] is null)
            return;

        var itemKey = ItemSlots[slotIdx].Item.itemName;
        EmptySlotIdxes.Add(slotIdx);
        SlotIdxes.GetValueOrDefault(itemKey).Remove(slotIdx);
        ItemSlots[slotIdx].CountChanged -= HandleItemCountChanged(slotIdx);
        ItemSlots[slotIdx] = null;
        SlotChanged?.Invoke(slotIdx);
    }

    private Action<int> HandleItemCountChanged(int slotIdx)
    {
        return count => SlotChanged?.Invoke(slotIdx);
    }
}