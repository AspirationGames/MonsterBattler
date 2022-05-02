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
    
    public void OnSave()
    {
        SavingSystem.i.Save("saveSlot1");

    }

    public void OnLoad()
    {
        SavingSystem.i.Load("saveSlot1");

    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if(GameController.Instance.GameState == GameState.Paused && context.performed)
        {
            Debug.Log("Exiting Menu");
            this.gameObject.SetActive(false);
            GameController.Instance.PauseGame(false);
        }
    }
}
