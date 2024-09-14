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
    private string _Name;
    public string Name { get { return _Name; } }

    [SerializeField]
    private string _Description;
    public string Description { get { return _Description; } }

    [SerializeField]
    private int _SlotSize;
    public int SlotSize { get { return _SlotSize; } }

    [SerializeField]
    private int _StackSize;
    public int StackSize { get { return _StackSize; } }

    [SerializeField]
    private bool _QuickAccess;
    public bool QuickAccess { get { return _QuickAccess; } }

    [SerializeField]
    private Sprite _Image;
    public Sprite Image { get { return _Image; } }

   
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
