using System.Threading;
using UnityEngine;
using Utilities;

public class RangeAbility : FlashlightAbility
{

    [SerializeField]
    private float range;

    [SerializeField]
    private float intensity;

    [SerializeField]
    private float rangeIncreaseRate;

    CountdownTimer timer;

    public override void OnUseAbility()
    {
        timer = new CountdownTimer(cooldown);
        timer.Start();

        _flashlight.Light.intensity = intensity;
        _flashlight.Light.color = Color.white;
        _flashlight.ConsumeBattery(cost);

    }

    public override void OnStopAbility()
    {
       
    }

    void Update()
    {
        if (timer != null)
        {
            timer.Tick(Time.deltaTime);

            // Gradually increase the range while the timer is running
            if (_flashlight.Light.range < range)
            {
                _flashlight.Light.range += rangeIncreaseRate * Time.deltaTime;

                // Ensure the range doesn't exceed the maximum allowed range
                if (_flashlight.Light.range > range)
                {
                    _flashlight.Light.range = range;
                }
            }

            if (timer.IsFinished)
            {
                _flashlight.ResetLight();
                timer = null;
            }
        }
    }
}
