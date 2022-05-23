using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, Interactable
{
    [SerializeField] ItemBase pickUpItem;

    public bool PickedUp {get; set;} = false;
    public IEnumerator Interact(Transform initiator)
    {
        if(!PickedUp)
        {
            initiator.GetComponent<Inventory>().AddItem(pickUpItem);

            PickedUp = true;
            
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            string playerName = initiator.GetComponent<PlayerController>().Name;

            yield return DialogManager.Instance.ShowDialogText($"{playerName} Found a {pickUpItem.ItemName}");
        }
        
        

    }

}
