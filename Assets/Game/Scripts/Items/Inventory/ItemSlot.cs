using System;
using System.Collections.Generic;

[Serializable]
public class ItemSlot
{
    private ItemSlot()
    {
    }

    public int Count { get; private set; }

    public ItemData Item { get; private set; }

    public bool IsFull => Count.Equals(Item.stackMaxCount);

    public int RemainingCount => Item.stackMaxCount - Count;


    public static ItemSlot CreateOne(ItemData item)
    {
        return new ItemSlot
        {
            Item = item,
            Count = 1
        };
    }

    public static List<ItemSlot> CreateMany(ItemData item, int count)
    {
        var i = new ItemSlot
        {
            Item = item
        };

        if (!i.TryAddItem(count, out var overflowItemSlots))
        {
            overflowItemSlots.Insert(0, i);
            return overflowItemSlots;
        }

        return new List<ItemSlot> { i };
    }


    /// <summary>
    /// </summary>
    /// <param name="count"></param>
    /// <returns> rest number after consuming</returns>
    public int FillUp(int count)
    {
        var rest = Count + count - Item.stackMaxCount;

        if (rest > 0)
        {
            Count = Item.stackMaxCount;
            return rest;
        }

        Count += count;
        return 0;
    }

    public bool TryAddItem(int count, out List<ItemSlot> overflowItemSlots)
    {
        overflowItemSlots = null;
        if (count + Count <= Item.stackMaxCount)
        {
            Count += count;
            return true;
        }

        overflowItemSlots = new List<ItemSlot>();
        var slotNumber = (count - RemainingCount) / Item.stackMaxCount;
        Count = Item.stackMaxCount;

        for (var i = 0; i < slotNumber; i++)
            overflowItemSlots.Add(new ItemSlot
            {
                Item = Item,
                Count = Item.stackMaxCount
            });

        var restNumber = count - Item.stackMaxCount * (slotNumber + 1);
        if (restNumber > 0)
            overflowItemSlots.Add(new ItemSlot
            {
                Item = Item,
                Count = restNumber
            });

        return false;
    }

    public int RemoveOne()
    {
        return --Count;
    }

    public void RemoveAll()
    {
        Count = 0;
    }

    public bool TryRemoveSome(int count)
    {
        if (count > Count)
            return false;

        Count -= count;
        return true;
    }
}