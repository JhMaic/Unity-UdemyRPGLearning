using CustomInspector;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] [SelfFill(true)] private SpriteRenderer sr;

    private void Awake()
    {
        sr.sprite = itemData.icon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Inventory.Instance.AutoFill(itemData, 13);
        Destroy(gameObject);
    }
}