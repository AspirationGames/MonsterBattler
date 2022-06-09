using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour, ISavable
{
    
    [SerializeField] ItemBase giveItem;
    [SerializeField] int giveItemQuantity = 1;
    [SerializeField] Dialog dialog;

    bool itemGiven = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        player.GetComponent<Inventory>().AddItem(giveItem, giveItemQuantity);
        itemGiven = true;

        string dialogText = $"{player.Name} recieved a {giveItem.ItemName}.";
        if(giveItemQuantity > 1)
        {
            dialogText = $"{player.Name} recieved {giveItemQuantity} {giveItem.ItemName}.";
        }

        yield return DialogManager.Instance.ShowDialogText(dialogText);    

    }

    public bool CanGiveItem()
    {
        return giveItem != null && giveItemQuantity > 0 && !itemGiven;
    }

    public object CaptureState()
    {
        return itemGiven;
    }

    public void RestoreState(object state)
    {
        itemGiven = (bool)state;
    }
}
