using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private InventorySlot _currentSlot;
    private InventorySlot _previousSlot;
    private InventoryItem _currentItem;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _currentItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        _currentSlot = _currentItem.GetComponentInParent<InventorySlot>();
        _previousSlot = _currentSlot;
        _currentItem.OnBeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _currentItem.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
/*        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
*/    }
}
