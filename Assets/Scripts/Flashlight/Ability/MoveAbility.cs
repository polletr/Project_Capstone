using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

public class MoveAbility : FlashlightAbility
{
    [SerializeField] private Transform moveHoldPos;
    [SerializeField] private float followSpeed;
    [SerializeField] private float range;

    [SerializeField] private MoveableObject pickup;

    private Vector3 movePos;

    private CountdownTimer timer;

    private void Pickup()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range) && hit.collider.TryGetComponent(out MoveableObject obj))
        {
            pickup = obj;
            pickup.transform.parent = moveHoldPos;
            pickup.rb.useGravity = false;
            pickup.rb.constraints = RigidbodyConstraints.FreezeRotation;
            _flashlight.ConsumeBattery(cost);
        }
        else
        {
            _flashlight.ResetLight();
            Debug.Log("No valid object to pick up.");
        }
    }

    private void Drop()
    {
        if (pickup != null)
        {
            pickup.transform.parent = null;
            pickup.rb.useGravity = true;
            pickup.rb.constraints = RigidbodyConstraints.None;
            pickup = null;
        }
    }

    private void FixedUpdate()
    {
        if (pickup != null)
        {
            //movePos = Vector3.Lerp(pickup.transform.position, moveHoldPos.position, followSpeed * Time.deltaTime);
            //pickup.rb.MovePosition(movePos);

            if (Vector3.Distance(pickup.transform.position, moveHoldPos.position) > 0.1f)
            {
                Vector3 directionToHoldPos = (moveHoldPos.position - pickup.transform.position).normalized;

                // Rotate the object to always face the player (or the hold position)
                Vector3 lookDirection = moveHoldPos.position - pickup.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(-lookDirection);  // Rotate to face the player
                pickup.rb.MoveRotation(Quaternion.Slerp(pickup.transform.rotation, targetRotation, followSpeed * Time.deltaTime));

                // Apply force to move the object towards the hold position
                pickup.rb.AddForce(directionToHoldPos * followSpeed, ForceMode.VelocityChange);
            }

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

        _flashlight.ResetLight();
    }
}
