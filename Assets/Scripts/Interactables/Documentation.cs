using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Documentation : Interactable
{
    [SerializeField] private Sprite document;
    private Animator animator;

    private readonly int openHash = Animator.StringToHash("Open");
    private readonly int closeHash = Animator.StringToHash("Close");
    
    private bool isOpen;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public override void OnInteract()
    {
        base.OnInteract();
        isOpen = !isOpen;
        DocumentUIHandler.Instance.MoveDocumentUI(document,isOpen);
        animator.Play(isOpen ? openHash : closeHash);
        var eventID = isOpen
            ? AudioManagerFMOD.Instance.SFXEvents.DocumentsOpen
            : AudioManagerFMOD.Instance.SFXEvents.DocumentsClose;
        AudioManagerFMOD.Instance.PlayOneShot(eventID, transform.position);
    }
}