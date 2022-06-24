using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, Interactable, ISavable
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

            AudioManager.i.PlaySFX(AudioID.ItemObtained, true);
            yield return DialogManager.Instance.ShowDialogText($"{playerName} Found a {pickUpItem.ItemName}");
        }
        
        

    }

    public object CaptureState()
    {
        return PickedUp;
    }

    public void RestoreState(object state)
    {
        PickedUp = (bool)state;

        if(PickedUp)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }


}
