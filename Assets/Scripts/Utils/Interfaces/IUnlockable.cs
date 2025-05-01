public interface IUnlockable
{
    int OpenID { get; }
    bool IsLocked { get; }

    void TryToUnlock();
    void Unlock();
    void StayLocked();
}