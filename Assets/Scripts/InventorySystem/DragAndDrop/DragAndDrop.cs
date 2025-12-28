using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private InventoryPresenter _presenter;
    [SerializeField] private GameObject _dragVisualObject;
    
    private InventoryView _myView;
    private static int _draggedSourceIndex = -1;

    private void Awake()
    {
        _myView = GetComponent<InventoryView>();
        if (_presenter == null) _presenter = FindObjectOfType<InventoryPresenter>();
    }

    
    public void OnBeginDrag(PointerEventData eventData)
    {
        
        var slotData = _presenter.InventoryModel.GetSlot(_myView.SlotIndex);
        if (slotData.IsEmpty) return;

        
        _draggedSourceIndex = _myView.SlotIndex;

       
       
        SetupDragVisual(slotData.Item.itemIcon, slotData.Quantity);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (_dragVisualObject != null && _dragVisualObject.activeSelf)
        {
            _dragVisualObject.transform.position = Input.mousePosition;
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (_draggedSourceIndex == -1) return;
        

        _presenter.InventoryModel.SwapItems(_draggedSourceIndex, _myView.SlotIndex);
        
        _draggedSourceIndex = -1;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
       
        if (_dragVisualObject != null)
        {
            _dragVisualObject.SetActive(false);
        }
        _draggedSourceIndex = -1; 
    }
    
    

    private void SetupDragVisual(Sprite icon, int qty)
    {
        _dragVisualObject.SetActive(true);
        _dragVisualObject.transform.position = Input.mousePosition;
        
        var img = _dragVisualObject.GetComponentInChildren<Image>();
        img.sprite = icon;
        img.raycastTarget = false; 

        var txt = _dragVisualObject.GetComponentInChildren<TextMeshProUGUI>();
        txt.text = qty > 1 ? qty.ToString() : "";
    }
}