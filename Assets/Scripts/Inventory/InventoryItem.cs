using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{

    [Header("UI Components")]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI quantityText;  

    public InventoryItemSO ItemSO;
    [field: SerializeField] public int Quantity { get; set; }  
    public Transform parentAfterDrag { get; set; }

    private void Awake()
    {

    }

    private void Start()
    {
        SetUpUI(ItemSO, Quantity);
        GetComponentInParent<InventorySlot>().CurrentItem = this;
    }

    public void SetUpUI(InventoryItemSO itemSO, int quantity)
    {
        ItemSO = itemSO;
        Quantity = quantity;

        image.sprite = itemSO.Icon; 

        quantityText.text = ItemSO.MaxStackSize > 1 ? Quantity.ToString() : "";
    }

    public void OnBeginDrag()
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;  
        transform.SetParent(transform.root); 
    }

    public void OnDrag()
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag()
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
