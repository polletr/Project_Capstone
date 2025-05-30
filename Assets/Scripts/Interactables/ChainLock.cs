using UnityEngine;
using UnityEngine.Events;

public class ChainLock : MonoBehaviour, IUnlockable
{
    public GameEvent Event;
    [field:SerializeField] public int OpenID { get; }
    [field: SerializeField] public bool IsLocked { get; private set; }
    
    public UnityEvent OnUnlockChainLock;
    
    public void Unlock()
    {
        //AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.UnlockDoor, transform.position);
        IsLocked = false;
        //disolve the lock? drop it
        OnUnlockChainLock?.Invoke();
        FinalExitManager.Instance.CheckChains();
    }
    
    public void TryToUnlock() => Event.OnTryToUnlock?.Invoke(this);    
    
    public void StayLocked()
    {
        //AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.EasyLockedDoor, transform.position);
        //shakeEffect.ShakeObject();
        //OnInteractLocked.Invoke();
    }
}
