using System;
using UnityEngine;

public class FlashLight : MonoBehaviour
{

    [field: SerializeField] public float Range { get; set; }
    [field: SerializeField] public Color LightColor { get; set; }
    [field: SerializeField] public float Intensity { get; set; }

    [field: SerializeField] public float Cost { get; set; }
    [field: SerializeField] public float BatteryLife { get; set; }


    [SerializeField] private FlashlightAbility[] flashlightAbilities;

    private FlashlightAbility currentAbility;

    public Light Light { get; set; }

    private void Awake()
    {
        Light = GetComponent<Light>();
        currentAbility = flashlightAbilities[0];
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward * Range);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 2f, Range);
        foreach (RaycastHit hit in hits)
        {
            var obj = hit.collider.gameObject;
            //obj.GetComponent<FlashlightStrategy>().Execute();
        }

        // Decrease BatteryLife continuously over time based on Cost per second
        BatteryLife -= Cost * Time.deltaTime;

        if (BatteryLife < 0)
        {
            BatteryLife = 0;
            // Add any logic for what happens when battery is depleted
            // Turn off the flashlight
        }
    }

    public void ResetLight(float cost)
    {
        Light.range = Range;
        Light.intensity = Intensity;
        Light.color = LightColor;
        BatteryLife -= cost;
    }

    public void HandleFlashAblility()
    {
        if (currentAbility != null)
            currentAbility.OnUseAbility();
    }

    public void ChangeSelectedAbility(int direction) // Fixed typo in method name
    {
        int currentIndex = Array.IndexOf(flashlightAbilities, currentAbility);


        // Update index based on direction (circular switching)
        currentIndex += direction;

        // Circular switching
        if (currentIndex >= flashlightAbilities.Length)
        {
            currentIndex = 0;
        }
        else if (currentIndex < 0)
        {
            currentIndex = flashlightAbilities.Length - 1;
        }

        // Update currentAbility to the new selected ability
        currentAbility = flashlightAbilities[currentIndex];
    }
}


public enum FlashlightAbilityType
{
    None,
    Range,
    Reveal,
    Move,

}


