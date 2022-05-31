using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour, PlayerControls.IMenuActions
{
     
    
    PlayerControls menuControls;

    
    private void Awake() 
    {
        menuControls = new PlayerControls();
        menuControls.Menu.SetCallbacks(this);    
    }

    void OnEnable() 
    {
        menuControls.Enable();
    }

    void OnDisable() 
    {
        menuControls.Disable();
    }
    
    //Inputs

    public void OnMonsterParty()
    {
        GameController.Instance.PartyManagement();
        
    }

    public void OnInventory()
    {
        GameController.Instance.ShowInventoryScreen();
        
    }
    public void OnSave()
    {
        SavingSystem.i.Save("saveSlot1");

    }

    public void OnLoad()
    {
        StartCoroutine( GameController.Instance.ReloadLastSave() );
        //SavingSystem.i.Load("saveSlot1");

    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if(GameController.Instance.GameState == GameState.Paused && context.performed)
        {
            //Exit Menu
            GameController.Instance.PauseGame(false);
            this.gameObject.SetActive(false);
            
        }
    }
}
