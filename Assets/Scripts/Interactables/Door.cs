using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField]
    private bool isLocked;
    [SerializeField]
    private UnityEvent OnOpen;
    [SerializeField]
    private UnityEvent OnUnlock;
    [SerializeField]
    private UnityEvent OnInteractLocked;

    [SerializeField]
    private GameObject PivotPoint;
    void OnInteract()
    {
        if (!isLocked)
        {
            OnOpen.Invoke();

        }
        else
        {
            //Check if player inventory has door key
            //if we dont have the key:
            OnInteractLocked.Invoke();
            //Buldgeoning sound and something that shows its locked
        }
    }

}
