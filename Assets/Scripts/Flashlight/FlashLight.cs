using UnityEngine;

public class FlashLight : MonoBehaviour
{

    public float Cost;
    public float Range;
    public Color LightColor;
    public float Intensity;
    public float BatteryLife;


    [SerializeField] private FlashlightAbility[] flashlightAbilities;

    private FlashlightAbility currentAbility;

    private Light _light;

    private RayCastTest detector;

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward * Range);
        RaycastHit[] hit = Physics.RaycastAll(ray, Range);
        foreach (RaycastHit h in hit)
        {
           var obj = h.collider.gameObject;
           /* if (obj.GetComponent<>())
            {
                obj.Execute();
            }*/
        }
    }

    private void DepleteBattery(FlashlightAbility ability)
    {
        BatteryLife -= ability.Cost;
    }


    public void HandleFlashAblility()
    {
        currentAbility.OnUseAbility();
    }
}


public enum FlashlightAbilityType
{
    None,
    Range,
    Reveal,
    Brightness
}


