using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class MoveAbility : FlashlightAbility 
{
    [SerializeField] private Transform moveHoldPos;
    [SerializeField] private float followSpeed;    
    [SerializeField] private float range;          

    private MoveableObject pickup;

    private CountdownTimer timer;

    private void Pickup()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range) && hit.collider.TryGetComponent(out MoveableObject obj))
        {
            pickup = obj;
            pickup.rb.useGravity = false;
        }
        else
        {
            _flashlight.ResetLight(0);
            Debug.Log("No valid object to pick up.");
        }
    }

    private void Drop()
    {
        if (pickup != null)
        {
            pickup.rb.useGravity = true;

            pickup = null;
        }
    }

    private void Update()
    {
        if (pickup != null)
        {
            pickup.transform.position = Vector3.Lerp(pickup.transform.position, moveHoldPos.position, followSpeed * Time.deltaTime);

            Vector3 directionToPlayer = (transform.position - pickup.transform.position).normalized;
            pickup.transform.rotation = Quaternion.LookRotation(-directionToPlayer); // Negative to make it face the player
        }
    }

    public override void OnUseAbility()
    {
        if (pickup == null)
            Pickup();
    }

    public override void OnStopAbility()
    {
        Drop();

        _flashlight.ResetLight(cost); 
    }
}
