using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryQuickAccess : MonoBehaviour
{
    [field: SerializeField] public Image icon;
    [field: SerializeField] public TextMeshProUGUI amountText;
    [field: SerializeField] public InventoryItemData item;
}
