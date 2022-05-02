using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    
    public void OnSave()
    {
        SavingSystem.i.Save("saveSlot1");

    }

    public void OnLoad()
    {
        SavingSystem.i.Load("saveSlot1");

    }


}
