using UnityEngine;

public class ChestHandler : MonoBehaviour, IInteractable
{
   [SerializeField] private int chestSize = 20;
   public InventoryModel ChestModel;
   [SerializeField] private ItemSO testItem;
   [SerializeField] private ItemSO testItem2;

   private void Awake()
   {
      ChestModel = new InventoryModel(chestSize);
      // For testing purposes, add some items to the chest
      ChestModel.AddItem(testItem, 3);
      ChestModel.AddItem(testItem2, 7);
   }

   public void Interact(GameObject interactor)
   {
     if (interactor.TryGetComponent<PlayerUIManager>(out var playerUIManager))
     {
        playerUIManager.OpenExternalStorage(ChestModel);
     }
         
   }
}
