using System;
using CustomInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour
{
    [SerializeField] [SelfFill(true)] private Image itemImage;

    [SerializeField] [SelfFill(true, mode = OwnerMode.DirectChildren)]
    private TextMeshProUGUI itemText;
    public ItemSlot item;

    private Color _defaultColor;
    private Sprite _defaultSprite;

    public int SlotIndex { get; set; }

    private void Awake()
    {
        _defaultColor = itemImage.color;
        _defaultSprite = itemImage.sprite;
    }

    public event Action<int> MouseDown;
    public event Action<int> MouseUp;

    public void InvokeMouseDown()
    {
        MouseDown?.Invoke(SlotIndex);
    }

    public void InvokeMouseUp()
    {
        MouseUp?.Invoke(SlotIndex);
    }

    private void SetDefault()
    {
        itemImage.color = _defaultColor;
        itemImage.sprite = _defaultSprite;
    }


    public void UpdateUI(ItemSlot newItem)
    {
        item = newItem;
        if (item != null)
        {
            itemImage.color = Color.white;
            itemImage.sprite = item.Item.icon;
            itemText.text = item.Count > 1 ? item.Count.ToString() : "";
        }
        else
        {
            SetDefault();
            itemText.text = "";
        }
    }
}