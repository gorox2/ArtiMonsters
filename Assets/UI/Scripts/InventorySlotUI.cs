using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text quantityText;
    public Button button;

    private ItemData currentItem;
    private int currentQty;
    private System.Action<ItemData, int> onClicked;

    public void Setup(ItemData item, int qty, System.Action<ItemData, int> onClick)
    {
        currentItem = item;
        currentQty = qty;
        onClicked = onClick;

        icon.sprite = item.icon;
        icon.enabled = item.icon != null;
        quantityText.text = qty > 1 ? $"x{qty}" : "";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClicked?.Invoke(currentItem, currentQty));
    }
}
