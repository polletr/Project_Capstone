using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    [Header("UI Components")]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI quantityText;  

    public InventoryItemData ItemSO;
    [field: SerializeField] public int Quantity { get; set; }  
    public Transform parentAfterDrag { get; set; }

    private void Awake()
    {
        SetUpUI(ItemSO, Quantity);
        GetComponentInParent<InventorySlot>().CurrentItem = this;
    }

    public void SetUpUI(InventoryItemData itemSO, int quantity)
    {
        ItemSO = itemSO;
        Quantity = quantity;

        image.sprite = itemSO.Image; 

        quantityText.text = ItemSO.StackSize > 1 ? Quantity.ToString() : "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;  
        transform.SetParent(transform.root); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }

    public void SetSlotParent(Transform parent)
    {
        parentAfterDrag = parent;
        transform.SetParent(parent);
    }
}
