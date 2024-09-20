using System.Threading;
using UnityEngine;
using Utilities;

public class RangeAbility : FlashlightAbility
{
    [SerializeField] private int cost;
    public override int Cost { get => cost; set => cost = value; }

    [SerializeField] private float cooldown;
    public override float Cooldown { get => cooldown; set => cooldown = value; }

    [SerializeField]
    private float range;

    [SerializeField]
    private float intensity;

    [SerializeField]
    private float rangeIncreaseRate;

    CountdownTimer timer;

    FlashLight flashlight;

    public override void OnUseAbility()
    {
        timer = new CountdownTimer(cooldown);
        timer.Start();
        flashlight = GetComponentInParent<FlashLight>();

        flashlight.Light.intensity = intensity;
        flashlight.Light.color = Color.white;
    }

    public override void OnStopAbility()
    {
        flashlight.ResetLight(Cost);
        GetComponentInParent<PlayerController>().ChangeState(new PlayerMoveState());
    }

    void Update()
    {
        if (timer != null)
        {
            timer.Tick(Time.deltaTime);

            // Gradually increase the range while the timer is running
            if (flashlight.Light.range < range)
            {
                flashlight.Light.range += rangeIncreaseRate * Time.deltaTime;

                // Ensure the range doesn't exceed the maximum allowed range
                if (flashlight.Light.range > range)
                {
                    flashlight.Light.range = range;
                }
            }

            if (timer.IsFinished)
            {
                OnStopAbility();
                timer = null;
            }
        }
    }
}
