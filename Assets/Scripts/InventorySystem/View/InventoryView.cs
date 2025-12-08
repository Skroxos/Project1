using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    
    public void SetData(ItemSO item, int quantity)
    {
        if (item != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = true;
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
        else
        {
            ClearData();
        }
    }
    
    private void ClearData()
    {
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        quantityText.text = "";
    }
}
