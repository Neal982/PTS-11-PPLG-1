using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public string itemName;
    public int itemQuantity;
    public Sprite itemIcon;
    public bool isFull;

    [SerializeField] private int maxStackSize = 99;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button slotButton;

    private ItemDetails itemDetails;
    private ItemDetailsPanel detailPanel;

    public bool IsEmpty => itemDetails == null;

    private void Awake()
    {
        if (IsEmpty)
        {
            if (itemImage != null)
            {
                itemImage.enabled = false;
            }

            if (quantityText != null)
            {
                quantityText.enabled = false;
            }
        }

        if (slotButton == null)
        {
            slotButton = GetComponentInChildren<Button>(true);
        }

        if (slotButton != null)
        {
            slotButton.onClick.AddListener(ShowDetails);
        }
    }

    public void SetDetailPanel(ItemDetailsPanel panel)
    {
        detailPanel = panel;
    }

    private void OnDestroy()
    {
        if (slotButton != null)
        {
            slotButton.onClick.RemoveListener(ShowDetails);
        }
    }

    public bool CanStackWith(ItemDetails item)
    {
        return !IsEmpty && !isFull && itemDetails == item;
    }

    public int GetSpaceFor(ItemDetails item)
    {
        if (item == null)
        {
            return 0;
        }

        if (IsEmpty)
        {
            return maxStackSize;
        }

        return itemDetails == item ? Mathf.Max(0, maxStackSize - itemQuantity) : 0;
    }

    public int AddItem(ItemDetails item, int amount)
    {
        if (item == null || amount <= 0 || GetSpaceFor(item) <= 0)
        {
            return 0;
        }

        if (IsEmpty)
        {
            itemDetails = item;
            itemName = item.ItemName;
            itemIcon = item.ItemIcon;
        }

        int addedAmount = Mathf.Min(amount, GetSpaceFor(item));
        itemQuantity += addedAmount;
        isFull = itemQuantity >= maxStackSize;

        if (quantityText != null)
        {
            quantityText.text = itemQuantity.ToString();
            quantityText.enabled = itemQuantity > 1;
        }

        if (itemImage != null)
        {
            itemImage.sprite = itemIcon;
            itemImage.enabled = true;
        }

        return addedAmount;
    }

    private void ShowDetails()
    {
        if (!IsEmpty && detailPanel != null)
        {
            detailPanel.Show(itemDetails, itemQuantity);
        }
    }

    private void OnValidate()
    {
        maxStackSize = Mathf.Max(1, maxStackSize);
    }
}
