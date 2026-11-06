using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemDetails ItemDetails;
    private GrabAbleItem itemCollector;
    
    void Awake()
    {
        foreach (MonoBehaviour component in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (component is GrabAbleItem collector)
            {
                itemCollector = collector;
                break;
            }
        }

        if (itemCollector == null)
        {
            Debug.LogError("No active component implementing GrabableItem was found in the scene.", this);
        }
    }

    public void SetItemDetails(ItemDetails itemDetails)
    {
        if (itemDetails == null)
        {
            Debug.LogError("Cannot initialize a pickup with null item details.", this);
            return;
        }

        ItemDetails = itemDetails;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ItemDetails != null && itemCollector != null)
        {
            if (itemCollector.AddItem(ItemDetails))
            {
                Destroy(gameObject);
            }
        }
    }
}