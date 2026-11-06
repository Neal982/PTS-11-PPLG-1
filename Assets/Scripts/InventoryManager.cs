using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour, GrabAbleItem
{
    private InputAction inventoryAction;
    [SerializeField] private GameObject InventoryCanvas;
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private ItemDetailsPanel itemDetailPanel;
    [SerializeField] private PlayerInput playerInput;
    private bool InventoryActive;
    [SerializeField] private ItemSlot[] itemSlot;
    private float previousTimeScale = 1f;
    private bool healthBarWasActive;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (InventoryCanvas != null)
        {
            InventoryCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("Inventory Canvas is not assigned.", this);
        }

        if (healthBarUI != null)
        {
            healthBarUI.SetActive(true);
            healthBarWasActive = true;
        }

        if (playerInput == null)
        {
            playerInput = FindFirstObjectByType<PlayerInput>();
        }

        if (playerInput != null)
        {
            inventoryAction = playerInput.actions.FindAction("Player/Inventory", false);
        }

        if (inventoryAction == null)
        {
            Debug.LogError("PlayerInput does not contain the Player/Inventory action.", this);
        }

        InventoryActive = false;
        if (itemSlot == null)
        {
            itemSlot = new ItemSlot[0];
        }

        foreach (ItemSlot slot in itemSlot)
        {
            if (slot != null)
            {
                slot.SetDetailPanel(itemDetailPanel);
            }
        }

        if (itemDetailPanel != null)
        {
            itemDetailPanel.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inventoryAction == null || !inventoryAction.WasPressedThisFrame() || InventoryCanvas == null)
        {
            return;
        }

        if (InventoryActive)
        {
            CloseInventory();
            return;
        }

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        InventoryActive = true;
        InventoryCanvas.SetActive(true);
        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (InventoryActive)
        {
            CloseInventory();
        }
    }

    private void CloseInventory()
    {
        InventoryActive = false;
        Time.timeScale = previousTimeScale;
        if (InventoryCanvas != null)
        {
            InventoryCanvas.SetActive(false);
        }

        if (healthBarUI != null)
        {
            healthBarUI.SetActive(healthBarWasActive);
        }
    }

    public bool AddItem(ItemDetails item)
    {
        if (item == null || item.Quantity <= 0 || itemSlot == null)
        {
            return false;
        }

        int availableSpace = 0;
        foreach (ItemSlot slot in itemSlot)
        {
            if (slot != null)
            {
                availableSpace += slot.GetSpaceFor(item);
            }
        }

        if (availableSpace < item.Quantity)
        {
            Debug.LogWarning("Inventory is full; item was not added.", this);
            return false;
        }

        int remaining = item.Quantity;
        foreach (ItemSlot slot in itemSlot)
        {
            if (slot != null && slot.CanStackWith(item))
            {
                remaining -= slot.AddItem(item, remaining);
            }
        }

        foreach (ItemSlot slot in itemSlot)
        {
            if (remaining > 0 && slot != null && slot.IsEmpty)
            {
                remaining -= slot.AddItem(item, remaining);
            }
        }

        if (remaining == 0)
        {
            Debug.Log("Item added to inventory: " + item.ItemName + " Quantity: " + item.Quantity);
            return true;
        }

        Debug.LogError("Inventory capacity changed while adding an item.", this);
        return false;
    }
}
