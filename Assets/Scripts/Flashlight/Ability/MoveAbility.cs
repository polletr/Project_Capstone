using UnityEngine;
using Utilities;

public class MoveAbility : FlashlightAbility
{
    [SerializeField] private Transform moveHoldPos;
    [SerializeField] private float pickupForce = 150f;
    [SerializeField] private float pickupRange = 10f;
    [SerializeField] private float maxHoldRange = 3f;
    [SerializeField] private float maxHoldTime = 15f;
    [SerializeField] private float pushForce = 15f;
    [SerializeField] private float pushCost = 10f;


    private MoveableObject pickup;

    private CountdownTimer timer;

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
        if (Physics.Raycast(_flashlight.transform.position, _flashlight.transform.forward, out RaycastHit hit, pickupRange) && hit.collider.TryGetComponent(out MoveableObject obj))
        {
            pickup = obj;

            pickup.Rb.useGravity = false;
            pickup.Rb.drag = 10;
            pickup.Rb.constraints = RigidbodyConstraints.FreezeRotation;

            pickup.transform.parent = moveHoldPos;
            pickup.transform.rotation = moveHoldPos.rotation;

            timer.Start();

            _flashlight.ConsumeBattery(Cost);
        }
        else
        {
            _flashlight.ResetLight();
            Debug.Log("No valid object to pick up.");
        }
    }

    private void Drop()
    {
        timer.Stop();
        timer.Reset();

        if (pickup != null)
        {
            pickup.transform.parent = null;
            pickup.Rb.useGravity = pickup.DefaultUseGravity;
            pickup.Rb.drag = pickup.DefaultDrag;
            pickup.Rb.constraints = pickup.DefaultConstraints;
            pickup = null;
        }
    }

    private void MoveObject()
    {

        if (pickup == null) return;

        float distance = Vector3.Distance(pickup.transform.position, moveHoldPos.position);

        if (distance > 0.1f)
        {
            Vector3 direction = moveHoldPos.position - pickup.transform.position;
            pickup.Rb.AddForce(direction.normalized * pickupForce);
        }

        if (distance > maxHoldRange || timer.IsFinished)
        {
            Drop();
        }
    }

    public void OnPushObj()
    {
        if (pickup != null)
        {
            timer.Stop();
            timer.Reset();

            pickup.transform.parent = null;
            pickup.Rb.useGravity = pickup.DefaultUseGravity;
            pickup.Rb.drag = pickup.DefaultDrag;
            pickup.Rb.constraints = pickup.DefaultConstraints;

            Vector3 direction =  pickup.transform.position - _flashlight.Player.transform.position;

            pickup.Rb.AddForce(direction.normalized * pushForce);

            _flashlight.ConsumeBattery(pushCost);

            pickup = null;
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

        _flashlight.ResetLight();
    }

}
