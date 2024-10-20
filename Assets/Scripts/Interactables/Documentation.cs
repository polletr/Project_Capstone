using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Documentation : Interactable
{
    [SerializeField] private Sprite document;

    public override void OnInteract()
    {
        base.OnInteract();
        DocumentUIHandler.Instance.MoveDocumentUI(document);
    }
}