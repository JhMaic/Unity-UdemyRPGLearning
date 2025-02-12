using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private List<int> _emptySlotIdxes;
    private List<ItemSlot> _itemSlots;

    /// <summary>
    ///     key: key of the item
    ///     value: slot index of the item
    /// </summary>
    private Dictionary<string, List<int>>　_slotIdxes;


    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        _itemSlots = new List<ItemSlot>();
        _slotIdxes = new Dictionary<string, List<int>>();
        _emptySlotIdxes = new List<int>();
    }

    private bool TryGetEmptySlotIdx(out int idx)
    {
        idx = 0;
        if (_emptySlotIdxes.Count == 0)
            return false;

        idx = _emptySlotIdxes.Min();
        return true;
    }

    private void AddItem(ItemData item, int count = 1)
    {
        if (!_slotIdxes.ContainsKey(item.itemName))
        {
            var slots = ItemSlot.CreateMany(item, count);
            AddToSlots(slots);
        }
    }

    public void AutoFill(ItemData item, int count = 1)
    {
        // if item is already has slots in inventory, check if it's fulfilled
        if (_slotIdxes.TryGetValue(item.itemName, out var idxes))
        {
            var restCount = count;


            foreach (var idx in idxes)
            {
                if (_itemSlots[idx].IsFull)
                    continue;

                restCount = _itemSlots[idx].FillUp(restCount);

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
        if (_slotIdxes.TryGetValue(keyName, out var value))
        {
            value.Add(idx);
            value.Sort();
            return;
        }

        _slotIdxes.Add(keyName, new List<int> { idx });
    }

    private void AddToSlots(ItemSlot itemSlot)
    {
        var hasEmptySlot = TryGetEmptySlotIdx(out var emptySlotIdx);

        if (!hasEmptySlot)
        {
            _itemSlots.Add(itemSlot);
            var idx = _itemSlots.Count - 1;
            UpdateSlotIdxes(itemSlot.Item.itemName, idx);
        }
        else
        {
            _itemSlots[emptySlotIdx] = itemSlot;
            _emptySlotIdxes.Remove(emptySlotIdx);
            UpdateSlotIdxes(itemSlot.Item.itemName, emptySlotIdx);
        }
    }

    private void AddToSlots(List<ItemSlot> itemSlots)
    {
        itemSlots.ForEach(AddToSlots);
    }

    public void ConsumeItem(int slotIdx)
    {
        var slot = _itemSlots[slotIdx];
        if (slot is null)
            return;

        var restCount = slot.RemoveOne();

        if (restCount == 0)
            RemoveSlot(slotIdx);
    }

    public bool TryConsumeItems(string keyName, int count)
    {
        var hasItem = _slotIdxes.TryGetValue(keyName, out var idxes);
        if (!hasItem)
            return false;

        var totalCount = idxes.Select(idx => _itemSlots[idx].Count).Sum();
        if (totalCount < count)
            return false;

        var restCount = count;
        foreach (var idx in idxes.ToList())
        {
            var slot = _itemSlots[idx];
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

    private void RemoveSlot(int slotIdx)
    {
        if (_itemSlots[slotIdx] is null)
            return;

        var itemKey = _itemSlots[slotIdx].Item.itemName;
        _emptySlotIdxes.Add(slotIdx);
        _slotIdxes.GetValueOrDefault(itemKey).Remove(slotIdx);
        _itemSlots[slotIdx] = null;
    }
}