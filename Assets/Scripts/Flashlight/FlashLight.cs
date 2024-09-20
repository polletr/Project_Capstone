using UnityEngine;

public class FlashLight : MonoBehaviour
{

    [field: SerializeField] public float Cost { get; set; }
    [field: SerializeField] public float Range { get; set; }
    [field: SerializeField] public Color LightColor { get; set; }
    [field: SerializeField] public float BatteryLife { get; set; }
    [field: SerializeField] public float Intensity { get; set; }


    [SerializeField] private FlashlightAbility[] flashlightAbilities;

    private FlashlightAbility currentAbility;

    private Light _light;

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward * Range);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 2f, Range);
        foreach (RaycastHit hit in hits)
        {
            var obj = hit.collider.gameObject;
            obj.GetComponent<FlashlightStrategy>().Execute();
        }
    }

    public void HandleFlashAblility()
    {
        if (currentAbility != null)
        currentAbility.OnUseAbility();
    }
}


public enum FlashlightAbilityType
{
    None,
    Range,
    Reveal,
    Move,

}


