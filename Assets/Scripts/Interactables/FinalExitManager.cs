using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FinalExitManager : Singleton<FinalExitManager>
{
    public GameEvent Event;
    [SerializeField] private List<IUnlockable> chains = new();
    
    public UnityEvent OnExitOpen;
    
    public void CheckChains()
    {
        foreach (var chain in chains)
        {
            if (chain.IsLocked)
                return;
        }
        OnExitOpen?.Invoke();
        Event.OnFinalExitOpen?.Invoke();
    }
}