using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public int itemID;
    public Sprite itemIcon;
    public int maxStackSize;
    public bool isStackable;
    
    public virtual void UseItem(GameObject user)
    {
        Debug.Log($"Using item: {itemName}");
    }
}
