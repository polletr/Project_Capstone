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
    private void Update()
    {
        if (playerController.HasFlashlight)
        {
            batteryLifeUI.text = flashLight.BatteryLife > 0 ? "Battery Life: " + flashLight.BatteryLife.ToString("F2") : flashLight.isActiveAndEnabled ? "Press R recharge" : "flash light off";
            currentAbilityUI.text = flashLight.CurrentAbility? flashLight.CurrentAbility.name : "No ability";
        }
        else
        {
            batteryLifeUI.text = "No flashlight";
            currentAbilityUI.text = "";
        }

        batteryCountUI.text = playerInventory? "Battery Collected: " + playerInventory.ChargesCollected : "No inventory setup";
        //playerHealthUI.text = playerController != null ? "Player Health: " + playerController.Health : "";

    }
}
