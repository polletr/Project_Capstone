using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryButton : MonoBehaviour
{
    [field: SerializeField] public Button button;
    [field: SerializeField] public Image icon;
    [field: SerializeField] public TextMeshProUGUI amountText;
    [field: SerializeField] public InventoryItemSO item;
}
