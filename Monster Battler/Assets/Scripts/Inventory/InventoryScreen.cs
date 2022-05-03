using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreen : MonoBehaviour
{
    
    public void OnBack()
    {
        GameController.Instance.CloseInventoryScreen();

    }

}
