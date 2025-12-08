using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private void Start()
    {
        ClearData();
    }

    public void SetData(Sprite item, int quantity)
    {
        if (item != null)
        {
            itemIcon.sprite = item;
            itemIcon.enabled = true;
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
        else
        {
            ClearData();
        }
    }
    
    public void ClearData()
    {
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        quantityText.text = "";
    }
}
