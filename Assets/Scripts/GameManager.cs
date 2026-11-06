using UnityEngine;

public class GameManager : MonoBehaviour, GrabAbleItem
{
    public static GameManager Instance;
    private InventoryManager inventory;

    private void Awake()
    {
        Instance = this;
        inventory = FindFirstObjectByType<InventoryManager>();
    }

    public bool AddItem(ItemDetails itemDetails)
    {
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<InventoryManager>();
        }

        if (inventory == null)
        {
            Debug.LogWarning("InventoryManager not found in the scene!");
            return false;
        }

        return inventory.AddItem(itemDetails);
    }
}