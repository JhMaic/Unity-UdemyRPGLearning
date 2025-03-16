using System;
using CustomInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler
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

    public void OnPointerDown(PointerEventData eventData)
    {
        MouseDown?.Invoke(SlotIndex);
    }

    public event Action<int> MouseDown;
    public event Action<int> MouseUp;


    public void SetDefault()
    {
        itemImage.color = _defaultColor;
        itemImage.sprite = _defaultSprite;
    }

    /// <summary>
    /// </summary>
    /// <param name="alpha"></param>
    /// <returns> restore function </returns>
    public Action SetOpacity(float alpha)
    {
        Action<float> setValue = value =>
        {
            var color = itemImage.color;
            color.a = value;
            itemImage.color = color;
        };
        ;
        var oldValue = itemImage.color.a;
        setValue(alpha);

        return () => setValue(oldValue);
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