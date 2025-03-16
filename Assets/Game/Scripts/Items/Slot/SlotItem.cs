public class SlotItem
{
    protected SlotItem()
    {
    }

    public ItemData Item { get; private set; }
    public int Count { get; private set; }

    public bool IsFull => Item.stackMaxCount == Count;

    public string Name => Item.itemName;

    public override string ToString()
    {
        return $"{Item.itemName}_{Count}";
    }


    public static SlotItem Create(ItemData itemData, int count, out int restCount)
    {
        if (itemData.stackMaxCount >= count)
        {
            restCount = 0;
            return new SlotItem
            {
                Item = itemData,
                Count = count
            };
        }

        restCount = count - itemData.stackMaxCount;
        return new SlotItem
        {
            Item = itemData,
            Count = itemData.stackMaxCount
        };
    }

    /// <summary>
    /// </summary>
    /// <param name="count"></param>
    /// <returns> how many has been added</returns>
    public int Add(int count)
    {
        var rest = Item.stackMaxCount - Count;
        if (rest > 0)
            if (rest >= count)
            {
                Count += count;
                return count;
            }
            else
            {
                Count = Item.stackMaxCount;
                return rest;
            }

        return 0;
    }


    /// <summary>
    /// </summary>
    /// <param name="count"></param>
    /// <returns> how many has been removed </returns>
    public int Remove(int count)
    {
        if (count > Count)
        {
            var removed = Count;
            Count = 0;
            return removed;
        }

        Count -= count;
        return count;
    }
}