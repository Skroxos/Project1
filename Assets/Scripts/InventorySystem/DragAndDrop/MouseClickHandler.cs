using UnityEngine;
using UnityEngine.EventSystems;

public class MouseClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] MouseSlotHandler mouseSlotHandler;
    
    
    private int firstSlotIndex;
    private int endSlotIndex;

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
