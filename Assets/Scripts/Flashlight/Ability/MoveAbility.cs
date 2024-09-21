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
            //pickup.rb.MoveRotation(Quaternion.Slerp(pickup.transform.rotation, Quaternion.LookRotation(directionToPlayer), followSpeed * Time.deltaTime));


            if (Vector3.Distance(pickup.transform.position, moveHoldPos.position) > 0.1f)
            {
                Vector3 directionToPlayer = (moveHoldPos.transform.position - pickup.transform.position).normalized;

                pickup.rb.AddForce(directionToPlayer);
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

        _flashlight.ResetLight(cost);
    }
}
