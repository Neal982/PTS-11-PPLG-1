using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailsPanel : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemQuantityText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    public void Show(ItemDetails item, int quantity)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        if (itemImage != null)
        {
            itemImage.sprite = item.ItemIcon;
            itemImage.enabled = true;
        }

        if (itemNameText != null) itemNameText.text = item.ItemName;
        if (itemQuantityText != null) itemQuantityText.text = quantity.ToString();
        if (itemDescriptionText != null) itemDescriptionText.text = item.Description;
    }

    public void Clear()
    {
        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }

        if (itemNameText != null) itemNameText.text = "";
        if (itemQuantityText != null) itemQuantityText.text = "";
        if (itemDescriptionText != null) itemDescriptionText.text = "";
    }
}