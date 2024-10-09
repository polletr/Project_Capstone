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
            batteryLifeUI.text = flashLight.battery != null ? "Battery Life: " + flashLight.battery.BatteryLife.ToString("F2") : flashLight.isActiveAndEnabled ? "No battery in flashlight press {Q}" : "pickup flash light";
            currentAbilityUI.text = flashLight.CurrentAbility != null ? "Current Ability: " + flashLight.CurrentAbility : "No ability";        }
        else
        {
            batteryLifeUI.text = "No flashlight";
            currentAbilityUI.text = "";
        }

        batteryCountUI.text = playerInventory != null ? "Battery Count: " + playerInventory.BatteryCount : "No inventory";
        //playerHealthUI.text = playerController != null ? "Player Health: " + playerController.Health : "";

    }
}
