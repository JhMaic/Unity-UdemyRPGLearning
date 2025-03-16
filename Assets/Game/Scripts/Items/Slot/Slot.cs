using System;
using R3;

public class Slot
{
    public enum Status
    {
        Empty,
        Full,
        Some
    }

    private IDisposable _disposable;

    public Slot(string slotId)
    {
        SlotId = slotId;
    }

    public SlotItem Item { get; private set; }
    public string SlotId { get; private set; }
    public Status Stat
    {
        get
        {
            if (Item == null)
                return Status.Empty;

            return Item.IsFull ? Status.Full : Status.Some;
        }
    }

    public static void SwapItem(Slot a, Slot b)
    {
        var tmpA = a.Item;
        var tmpB = b.Item;

        a.RemoveItem();
        b.RemoveItem();

        try
        {
            a.PutItem(tmpB);
            b.PutItem(tmpA);
        }
        catch (Exception e)
        {
            a.PutItem(tmpA);
            b.PutItem(tmpB);
        }
    }

    public event Action ChangeEvent;

    public virtual void PutItem(SlotItem item)
    {
        if (item == null)
        {
            // null is treated as removing
            RemoveItem();
            return;
        }

        if (Item != null)
            // if already has, remove it
            RemoveItem();

        Item = item;

        _disposable = Observable.EveryValueChanged(this, x => x.Item.Count)
            .Subscribe(_ =>
            {
                if (Item.Count == 0)
                    RemoveItem();
                else
                    ChangeEvent?.Invoke();
            });

        ChangeEvent?.Invoke();
    }

    public void RemoveItem()
    {
        if (Item == null)
            return;

        Item = null;

        _disposable.Dispose();
        ChangeEvent?.Invoke();
    }
}