using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour , IInventoryItem
{

    [SerializeField]
    private float damage;
    public float Damage { get { return damage; } }

    [SerializeField, Range(0.2f, 5f)]
    private float attackRange = 2f;
    public float AttackRange { get { return attackRange; } }

    [SerializeField]
    private InventoryItemSO _ItemSO;
    public InventoryItemSO ItemSO { get { return _ItemSO; } }
   
    public void OnPickup()
    {
        gameObject.SetActive(false);

    }
    public void OnDrop()
    {
        transform.SetParent(null);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Enable the Rigidbody if it's not already enabled
            rb.isKinematic = false;

            // Apply a force to make it fall or be thrown
            rb.AddForce(Vector3.forward * 5, ForceMode.Impulse);
        }

    }


}
