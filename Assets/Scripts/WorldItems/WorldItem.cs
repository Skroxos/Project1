using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractable
{
   [SerializeField] private ItemSO itemData;
    [SerializeField] private int quantity = 1;
   
    public void Interact(GameObject interactor) 
    {
        if (interactor.TryGetComponent(out InventoryPresenter inventory))
        {
            Debug.Log("Picking up item: " + itemData.itemName + " Quantity: " + quantity);
           int leftover = inventory.AddItemToInventory(itemData, quantity);
           if (leftover <= 0)
           {
                Destroy(gameObject);
           }
           else
           {
               quantity = leftover;
               Debug.Log("Not enough space in inventory. Leftover quantity: " + leftover);
           }
        }
        
        
    }
}
