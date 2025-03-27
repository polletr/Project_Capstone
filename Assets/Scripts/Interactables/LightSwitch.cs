using UnityEngine;

public class LightSwitch : Interactable
{
    [SerializeField] private LightController[] lightControllers;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void TurnLightState()
    {
        // Read current state from the animator
        bool currentState = animator.GetBool("IsOn");

        // Toggle the animator parameter
        animator.SetBool("IsOn", !currentState);

        foreach (LightController light in lightControllers)
        {
            light.TurnOnOffLight(!light.lightSource.enabled);
        }
    }
}
