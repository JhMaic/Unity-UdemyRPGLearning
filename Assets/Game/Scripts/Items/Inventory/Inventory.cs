using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [SerializeField]
    public int maxSlotCount = 64;

    public List<int> EmptySlotIdxes { get; private set; }
    public List<ItemSlot> ItemSlots { get; private set; }

    /// <summary>
    ///     key: key of the item
    ///     value: slot index of the item
    /// </summary>
    public Dictionary<string, List<int>> SlotIdxes { get; private set; }

    [CanBeNull] public ItemSlot MouseSlot { get; set; }


    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        ItemSlots = Enumerable.Repeat<ItemSlot>(null, maxSlotCount).ToList();
        SlotIdxes = new Dictionary<string, List<int>>();
        EmptySlotIdxes = Enumerable.Range(0, maxSlotCount).ToList();
        MouseSlot = null;
    }

    /// <summary>
    ///     create/remove/update
    ///     slot_idx
    /// </summary>
    public event Action<int> SlotChanged;

    /// <summary>
    ///     place items automatically, e.g, picking up;
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
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

    /// <summary>
    ///     consume some items by slot idx
    /// </summary>
    /// <param name="slotIdx"></param>
    /// <param name="count"></param>
    public void ConsumeItem(int slotIdx, int count = 1)
    {
        var slot = ItemSlots[slotIdx];
        if (slot is null)
            return;

        if (slot.TryRemoveSome(count))
            if (slot.Count.Equals(0))
                RemoveSlot(slotIdx);
    }

    /// <summary>
    ///     consume all item by slot idx
    /// </summary>
    /// <param name="slotIdx"></param>
    public void ConsumeAllItems(int slotIdx)
    {
        var slot = ItemSlots[slotIdx];
        if (slot is null)
            return;

        slot.RemoveAll();
        RemoveSlot(slotIdx);
    }

    /// <summary>
    ///     consume many items by name
    /// </summary>
    /// <param name="keyName"> item name </param>
    /// <param name="count"> how many to consume </param>
    /// <returns></returns>
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

    public void SwapPosition(int slotAIdx, int slotBIdx)
    {
        var slotA = ItemSlots[slotAIdx];
        var slotB = ItemSlots[slotBIdx];

        if (slotA is null)
            return;

        if (slotB is null)
        {
            AddToSlots(slotA, slotBIdx);
            RemoveSlot(slotAIdx);
        }
        else
        {
            if (slotB.Item.itemName.Equals(slotA.Item.itemName))
            {
                // same items
                var rest = slotB.FillUp(slotA.Count);
                ConsumeItem(slotAIdx, slotA.Count - rest);
            }
            else
            {
                // different items
                RemoveSlot(slotAIdx);
                RemoveSlot(slotBIdx);
                AddToSlots(slotB, slotAIdx);
                AddToSlots(slotA, slotBIdx);
            }
        }
    }

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
            throw new Exception("No spare slot!");
        }

        SlotChanged?.Invoke(idx);
        itemSlot.CountChanged += HandleItemCountChanged(idx);
    }

    private void AddToSlots(ItemSlot itemSlot, int targetIdx)
    {
        if (ItemSlots[targetIdx] is not null)
            return;

        ItemSlots[targetIdx] = itemSlot;
        EmptySlotIdxes.Remove(targetIdx);
        UpdateSlotIdxes(itemSlot.Item.itemName, targetIdx);

        SlotChanged?.Invoke(targetIdx);
        itemSlot.CountChanged += HandleItemCountChanged(targetIdx);
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