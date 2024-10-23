using UnityEngine;
using Utilities;

public class MoveAbility : FlashlightAbility
{
    [SerializeField] private float maxHoldRange = 3f;
    [SerializeField] private float maxHoldTime = 15f;
    [SerializeField] private float pickupForce = 150f;
    [field: SerializeField] public float PickupRange { get; private set; } = 10f;

    private MoveableObject pickup;
    private CountdownTimer timer;
    private Transform moveHoldPos;

    private void Awake()
    {
        timer = new CountdownTimer(maxHoldTime);
    }

    public override void Initialize(FlashLight flashlight)
    {
        base.Initialize(flashlight);
        moveHoldPos = flashlight.MoveHoldPos;
    }
    
    private void Pickup()
    {
        if (Physics.Raycast(Flashlight.RayCastOrigin.position, Flashlight.RayCastOrigin.forward, out var hit, PickupRange) &&
            hit.collider.TryGetComponent(out MoveableObject obj))
        {
            pickup = obj;
            pickup.BoxOutline.enabled = true;


            pickup.Rb.useGravity = false;
            pickup.Rb.drag = 10;
            pickup.Rb.constraints = RigidbodyConstraints.FreezeRotation;

            pickup.transform.parent = moveHoldPos;
            pickup.transform.rotation = moveHoldPos.rotation;

            timer.Start();

            Flashlight.ConsumeBattery(Cost);
        }
        else
        {
            Flashlight.ResetLight();
            Debug.Log("No valid object to pick up.");
        }
    }

    private void Drop()
    {
        timer.Stop();
        timer.Reset();

        if (pickup == null) return;
        pickup.transform.parent = null;
        pickup.Rb.useGravity = pickup.DefaultUseGravity;
        pickup.Rb.drag = pickup.DefaultDrag;
        pickup.Rb.constraints = pickup.DefaultConstraints;
        pickup = null;
    }

    private void MoveObject()
    {
        if (pickup == null) return;

        var distance = Vector3.Distance(pickup.transform.position, moveHoldPos.position);

        if (distance > 0.1f)
        {
            var direction = moveHoldPos.position - pickup.transform.position;
            pickup.Rb.AddForce(direction.normalized * pickupForce);
        }

        if (distance > maxHoldRange || timer.IsFinished)
        {
            Drop();
        }
    }

    private void FixedUpdate()
    {
        MoveObject();
    }

    private void Update() => timer.Tick(Time.deltaTime);

    public override void OnUseAbility()
    {
        if (pickup == null)
            Pickup();
    }

    public override void OnStopAbility()
    {
        Drop();

        Flashlight.ResetLight();
    }
}