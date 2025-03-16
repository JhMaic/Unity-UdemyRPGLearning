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

    private Color _defaultColor;
    private Sprite _defaultSprite;
    private Slot _slot;

    private GameContext.RuntimeContext Runtime => GameContext.Instance.Runtime;


    private void Awake()
    {
        _defaultColor = itemImage.color;
        _defaultSprite = itemImage.sprite;
    }

    private void OnDestroy()
    {
        _slot.ChangeEvent -= OnSlotChanged;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Runtime.inventory.isMouseHolding)
        {
            if (_slot.Stat == Slot.Status.Empty)
                return;

            // not holding any slot
            SetOpacity(0.5f);
            Runtime.inventory = new GameContext.RuntimeContext.Inventory
            {
                isMouseHolding = true,
                heldSlot = this
            };
        }
        else
        {
            // is holding a slot
            Slot.SwapItem(Runtime.inventory.heldSlot._slot, _slot);
            Runtime.inventory = new GameContext.RuntimeContext.Inventory
            {
                isMouseHolding = false,
                heldSlot = null
            };
        }
    }


    public void Init(Slot slot)
    {
        _slot = slot;
        _slot.ChangeEvent += OnSlotChanged;
    }

    private void OnSlotChanged()
    {
        UpdateUI();
    }

    public void SetDefault()
    {
        itemImage.color = _defaultColor;
        itemImage.sprite = _defaultSprite;
    }

    /// <summary>
    /// </summary>
    /// <param name="alpha"></param>
    /// <returns> restore function </returns>
    public void SetOpacity(float alpha)
    {
        void SetValue(float value)
        {
            var color = itemImage.color;
            color.a = value;
            itemImage.color = color;
        }

        SetValue(alpha);
    }


    public void UpdateUI()
    {
        if (_slot.Item != null)
        {
            itemImage.color = Color.white;
            itemImage.sprite = _slot.Item.Item.icon;
            itemText.text = _slot.Item.Count > 1 ? _slot.Item.Count.ToString() : "";
        }
        else
        {
            SetDefault();
            itemText.text = "";
        }
    }
}