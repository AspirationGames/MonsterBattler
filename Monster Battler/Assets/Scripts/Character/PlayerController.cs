using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions, ISavable
{
    [SerializeField] string playerName;
    [SerializeField] Sprite sprite;
    Vector2 moveDirection;
    PlayerControls playerControls;
    Character character;
    
    [SerializeField] PauseMenu pauseMenu;
    void Awake() 
    {  
        character = GetComponent<Character>();

        playerControls = new PlayerControls();
        playerControls.Player.SetCallbacks(this); 
    }

    void OnEnable() 
    {
        playerControls.Enable();
    }

    void OnDisable() 
    {
        playerControls.Disable();
    }
   
    public void OnMove(InputAction.CallbackContext context)
    {

        if (GameController.Instance.GameState != GameState.OverWorld) 
        {
            moveDirection = Vector2.zero; //fixed bug where character would move after battle
            return;
        }

        if(context.started)
        {
            return;
        }
        
    
        moveDirection = context.ReadValue<Vector2>();
        
    }

    

    public void OnInteract(InputAction.CallbackContext context)
    {   
        if(GameController.Instance.GameState != GameState.OverWorld)
        {
            return;
        }
        if(context.started)
        {
            return;
        }
        else if(context.performed)
        {
            Interact();
        }
            
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (GameController.Instance.GameState != GameState.OverWorld) 
        {
            return;
        }
        if(context.started)
        {
            return;
        }
        else if(context.performed)
        {
            pauseMenu.gameObject.SetActive(true);
            GameController.Instance.PauseGame(true);
        }
        
    }

    

    public void HandleUpdate()
    {

         if(!character.IsMoving && moveDirection != Vector2.zero)
        {
            

            StartCoroutine( character.Move(moveDirection, RunAfterMove) );
            
        }

        character.HandleUpdate();
        
    }

    void Interact()
    {
        var faceDirection = new Vector3(character.CharacterAnimator.MoveX, character.CharacterAnimator.MoveY);

        var interactPosition = transform.position + faceDirection;

        Debug.DrawLine(transform.position, interactPosition, Color.green, 1f);

        var interactableCollider = Physics2D.OverlapCircle(interactPosition, 0.3f, GameLayers.i.InteractableLayer);
        
        if (interactableCollider != null)
        {
            interactableCollider.GetComponent<Interactable>()?.Interact(transform); //finds Interactable interace. Any interactable items should have this interface attached to their class
        }

    }

    void RunAfterMove()
    {
      var triggerableColliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0,character.OffSetY), 0.2f, GameLayers.i.TriggerableLayers); //offset y is used in this case to prefent detecting collision above palyer incorrectly

      foreach (var collider in triggerableColliders)
      {
        var triggerable = collider.GetComponent<IPlayerTriggerable>();
        if(triggerable != null)
        {
            triggerable.OnPlayerTriggered(this);
            break;
        }
      }
    }

    public object CaptureState()
    {
        //save data
        var saveData = new PlayerSaveData()
        {
            savedPosition = new float[] {transform.position.x, transform.position.y},
            savedMonsters = GetComponent<MonsterParty>().Monsters.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;

    }

    public void RestoreState(object state)
    {
        //load data
        var saveData = (PlayerSaveData)state;

        //Restore Player Position
        var position = saveData.savedPosition;
        transform.position = new Vector3 (position[0],position[1]);

        //Restore Monster Party
        GetComponent<MonsterParty>().Monsters = saveData.savedMonsters.Select(s => new Monster(s)).ToList(); //transformes each monster save data back into monsters.
    }

    

    public string Name
    {
        get => playerName;
    }

    public Sprite Sprite
    {
        get => sprite;
    }

    public Character Character
    {
        get => character;
    }
}

[Serializable]
public class PlayerSaveData
{
    public float[] savedPosition;
    public List<MonsterSaveData> savedMonsters;
}