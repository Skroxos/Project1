using UnityEngine;

public class ItemSlot
{
    public int Quantity { get; set; }
    public ItemSO Item { get; set; }
    
    public bool IsEmpty => Item == null || Quantity <= 0;
    public bool IsFull => Item != null && Quantity >= Item.maxStackSize;
    
    public void Clear()
    {
        Item = null;
        Quantity = 0;
    }
}
