using UnityEngine;

public class RangeAbility : FlashlightAbility
{
    [SerializeField] private int cost;
    public override int Cost { get => cost; set => cost = value; }

    [SerializeField] private float cooldown;
    public override float Cooldown { get => cooldown; set => cooldown = value; }


    public override void OnUseAbility()
    {

    }

    public override void OnStopAbility()
    {

    }

    void Update()
    {

    }
}
