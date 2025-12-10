using System;
using System.Collections.Generic;


public class InventoryModel
{
    public int Capacity { get; private set; }
    private readonly List<ItemSlot> _slots;
    public event Action<int> OnSlotChanged;
    
    public InventoryModel(int capacity)
    {
        Capacity = capacity;
        _slots = new List<ItemSlot>();
        for (int i = 0; i < capacity; i++)
        {
            _slots.Add(new ItemSlot());
        }
    }
    
    public ItemSlot GetSlot(int index) => _slots[index];
    public void SwapItems(int indexA, int indexB)
    {
        var temp = _slots[indexA];
        _slots[indexA] = _slots[indexB];
        _slots[indexB] = temp;
        OnSlotChanged?.Invoke(indexA);
        OnSlotChanged?.Invoke(indexB);
    }
    
    public int AddItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < Capacity; i++)
        {
            var slot = _slots[i];
            if (!slot.IsEmpty && slot.Item.itemID == item.itemID && item.isStackable)
            {
                int spaceLeft = item.maxStackSize - slot.Quantity;
                int toAdd = Math.Min(spaceLeft, quantity);
                slot.Quantity += toAdd;
                quantity -= toAdd;
                OnSlotChanged?.Invoke(i);
                if (quantity <= 0) return 0;
            }
        }
        
        for (int i = 0; i < Capacity; i++)
        {
            var slot = _slots[i];
            if (slot.IsEmpty)
            {
                int toAdd = Math.Min(item.maxStackSize, quantity);
                slot.Item = item;
                slot.Quantity = toAdd;
                quantity -= toAdd;
                OnSlotChanged?.Invoke(i);
                if (quantity <= 0) return 0;
            }
        }
        
        return quantity; 
        
    }

   public void RemoveItem(int index, int quantity)
   {
       var slot = _slots[index];
       if (slot.IsEmpty) return;
       
       slot.Quantity -= quantity;
       if (slot.Quantity <= 0)
       {
           slot.Clear();
       }
       OnSlotChanged?.Invoke(index);
   }
}
