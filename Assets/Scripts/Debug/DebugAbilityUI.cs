using TMPro;
using UnityEngine;

public class DebugAbilityUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI batteryLifeUI;
    [SerializeField] private TextMeshProUGUI batteryCountUI;
    [SerializeField] private TextMeshProUGUI currentAbilityUI;
    [SerializeField] private TextMeshProUGUI playerHealthUI;

    [SerializeField] private FlashLight flashLight;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerController playerController;
    // Update is called once per frame
    void Update()
    {
        if (flashLight != null)
        {
            batteryLifeUI.text = flashLight.BatteryLife > 0 ? "Battery Life: " + flashLight.BatteryLife.ToString("F2") : flashLight.isActiveAndEnabled ? "press {Q} to recharge" : "pickup flash light";
            currentAbilityUI.text = flashLight.CurrentAbility != null ? "Current Ability: " + flashLight.CurrentAbility : "No ability";        }
        else
        {
            batteryLifeUI.text = "No flashlight";
            currentAbilityUI.text = "";
        }

        batteryCountUI.text = playerInventory != null ? "BatteryPack Count: " + playerInventory.ChargesCollected : "No inventory setup";
        //playerHealthUI.text = playerController != null ? "Player Health: " + playerController.Health : "";

    }
}
