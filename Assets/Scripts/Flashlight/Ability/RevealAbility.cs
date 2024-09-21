public class RevealAbility : FlashlightAbility
{
    public override void OnStopAbility()
    {
      _flashlight.ResetLight(cost);
    }

    public override void OnUseAbility()
    {
         OnStopAbility();
    }

}
