using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPresenter : MonoBehaviour
{
    [SerializeField] private GameObject[] slotGameObjects;
    [SerializeField] private int maxInventorySize = 20;
    private InventoryModel inventoryModel;
    private List<InventoryView> inventoryViews;
    
    [SerializeField] ItemSO testItem;
    [SerializeField] ItemSO testItem2;


    private void Start()
    {
        inventoryModel = new InventoryModel(maxInventorySize);
        inventoryViews = new List<InventoryView>();
        for (int i = 0; i < slotGameObjects.Length; i++)
        {
            InventoryView slotView = slotGameObjects[i].GetComponent<InventoryView>();
            inventoryViews.Add(slotView);
        }
        inventoryModel.OnSlotChanged += UpdateSlotView;
    }

    private void UpdateSlotView(int obj)
    {
        var slotData = inventoryModel.GetSlot(obj);
        var view = inventoryViews[obj];
        
        if (slotData.IsEmpty)
        {
            view.ClearData();
        }
        else
        {
            view.SetData(slotData.Item.itemIcon, slotData.Quantity);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventoryModel.AddItem(testItem, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventoryModel.AddItem(testItem2, 5);
        }
    }
    
    public int AddItemToInventory(ItemSO item, int quantity)
    {
        int leftover = inventoryModel.AddItem(item, quantity);
        return leftover;
    }
}
