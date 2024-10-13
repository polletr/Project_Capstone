public class Battery : Interactable , ICollectable
{
    public void Collect()
    {
        //pool manager handle collected 
        gameObject.SetActive(false);
    }
    
    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
    }
}
