using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbility : MonoBehaviour
{
    [SerializeField] private Transform moveHoldPos;
    [SerializeField] private float followSpeed;    
    [SerializeField] private float range;          

    private GameObject pickupObj;                  
    private Rigidbody pickupRb;                    

    private void Pickup()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range) &&
            hit.collider.CompareTag("Pickup"))
        {
            Debug.Log($"Picked up {hit.collider.gameObject.name}");
            pickupObj = hit.collider.gameObject;

            pickupRb = pickupObj.GetComponent<Rigidbody>();

            pickupRb.useGravity = false;
            pickupRb.freezeRotation = true;

        }
        else
        {
            Debug.Log("No valid object to pick up.");
        }
    }

    private void Drop()
    {
        if (pickupObj != null)
        {
            pickupRb.useGravity = true;
            pickupRb.freezeRotation = false;

            pickupObj = null;
            pickupRb = null;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (pickupObj == null)
            {
                Pickup();
            }
            else
            {
                pickupObj.transform.position = Vector3.Lerp(pickupObj.transform.position, moveHoldPos.position, followSpeed * Time.deltaTime);
                Vector3 directionToPlayer = (transform.position - pickupObj.transform.position).normalized;
                pickupObj.transform.rotation = Quaternion.LookRotation(-directionToPlayer); // Negative to make it face the player
            }
        }
        else
        {
            Drop();
        }
    }
}
