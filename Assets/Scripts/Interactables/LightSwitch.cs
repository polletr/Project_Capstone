using UnityEngine;

public class LightSwitch : Interactable
{
    [SerializeField] private LightController[] lightControllers;
    public void TurnLightState()
    {
        foreach (LightController light  in lightControllers)
        {
            light.TurnOnOffLight(!light.lightSource.enabled);
        }
    }

}
