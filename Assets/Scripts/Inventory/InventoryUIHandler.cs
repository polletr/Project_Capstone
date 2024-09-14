using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUIHandler : MonoBehaviour
{
    [SerializeField]
    private List<InventoryButton> inverntorybuttons = new();

    public GameEvent Event;

    private void OnEnable()
    {
        Event.OnItemAdded += PopulateUI;
        Event.OnItemRemoved += PopulateUI;
    }

    private void OnDisable()
    {
        Event.OnItemAdded -= PopulateUI;
        Event.OnItemRemoved -= PopulateUI;

    }

    public void PopulateUI(IInventoryItem item, Dictionary<IInventoryItem, int> itemDictionary)
    {

        foreach (InventoryButton button in inverntorybuttons)
        {
            button.item = null;
            button.icon.sprite = null;
            button.amountText.text = "";
        }

       int i = 0;
        foreach (KeyValuePair<IInventoryItem, int> obj in itemDictionary)
        {
            inverntorybuttons[i].item = obj.Key;

            if (obj.Value > 1)
                inverntorybuttons[i].amountText.text = obj.Value.ToString();
            else
                inverntorybuttons[i].amountText.text = "";

            inverntorybuttons[i].icon.sprite = obj.Key.Image;

            i++;

        }
    }

    // add description to the inventory popup 
}

