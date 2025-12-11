using System;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private ExternalStoragePresenter _externalStoragePresenter;
    [SerializeField] private InventoryPresenter _playerInventoryPresenter;
    
    public void OpenExternalStorage(InventoryModel storageModel)
    {
        _externalStoragePresenter.ConnectToStorageModel(storageModel);
        
    }

    public void CloseAll()
    {
        _externalStoragePresenter.DisconnectStorageModel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAll();
        }
    }
}
