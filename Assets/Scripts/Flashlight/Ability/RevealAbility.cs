public class RevealAbility : FlashlightAbility
{
    public override void OnStopAbility()
    {
      _flashlight.ResetLight();
    }

    public override void OnUseAbility()
    {
         OnStopAbility();
    }

}
