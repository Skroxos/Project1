using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    
    public int SlotIndex { get; set; }

    public void SetData(Sprite item, int quantity)
    {
        if (item != null)
        {
            var tempColor = itemIcon.color;
            tempColor.a = 1f;
            itemIcon.color = tempColor;
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
    public Sprite GetItemIcon()
    {
        return itemIcon.sprite;
    }
    public int GetItemQuantity()
    {
        if (int.TryParse(quantityText.text, out int quantity))
        {
            return quantity;
        }
        return 1;
    }
}


