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


    public void UpdateUI(ItemSlot newItem)
    {
        item = newItem;
        if (item != null)
        {
            itemImage.sprite = item.Item.icon;
            itemText.text = item.Count > 1 ? item.Count.ToString() : "";
        }
        else
        {
            itemImage.sprite = null;
            itemText.text = "";
        }
    }
}