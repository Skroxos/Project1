using System;
using System.Collections.Generic;
using UnityEngine;

public class ExternalStoragePresenter : MonoBehaviour
{
    [SerializeField] private GameObject _externalStorageUI;
    private InventoryModel _currentStorageModel;
    private List<InventoryView> _storageViews;

    private void Awake()
    {
        _externalStorageUI.SetActive(false);
        _storageViews = new List<InventoryView>();
        foreach (Transform slot in _externalStorageUI.transform)
        {
            InventoryView slotView = slot.GetComponentInChildren<InventoryView>();
            if (slotView != null)
            {
                _storageViews.Add(slotView);
            }
        }
       
    }
    
    public void ConnectToStorageModel(InventoryModel storageModel)
    {
        if (_currentStorageModel != null)
        {
            _currentStorageModel.OnSlotChanged -= UpdateStorageSlotView;
        }
        _currentStorageModel = storageModel;
        _currentStorageModel.OnSlotChanged += UpdateStorageSlotView;
        _externalStorageUI.SetActive(true);
        for (int i = 0; i < _currentStorageModel.Capacity; i++)
        {
            UpdateStorageSlotView(i);
        }
        
    }
    
    public void DisconnectStorageModel()
    {
        if (_currentStorageModel != null)
        {
            _currentStorageModel.OnSlotChanged -= UpdateStorageSlotView;
            _currentStorageModel = null;
        }
        _externalStorageUI.SetActive(false);
    }

    private void UpdateStorageSlotView(int obj)
    {
        var slotData = _currentStorageModel.GetSlot(obj);
        var view = _storageViews[obj];
        
        if (slotData.IsEmpty)
        {
            view.ClearData();
        }
        else
        {
            view.SetData(slotData.Item.itemIcon, slotData.Quantity);
        }
    }
}
