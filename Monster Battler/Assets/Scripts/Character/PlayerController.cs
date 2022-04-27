using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions 
{
    [SerializeField] string playerName;
    [SerializeField] Sprite sprite;

    const float offsetY = 0.3f; //this offset will account for detecting collision inappropriatly due to player sprite not being at center of tile.
    Vector2 moveDirection;
    PlayerControls playerControls;
    Character character;
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

    public bool IsInFreeRoamState()
    {
        if(GameController.Instance.GameState == GameState.OverWorld)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
   
    public void OnMove(InputAction.CallbackContext context)
    {

        if (!IsInFreeRoamState()) {
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
        if(!IsInFreeRoamState())
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
      var triggerableColliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0,offsetY), 0.2f, GameLayers.i.TriggerableLayers);

      foreach (var collider in triggerableColliders)
      {
        var triggerable = collider.GetComponent<IPlayerTriggerable>();
        if(triggerable != null)
        {
            character.CharacterAnimator.IsMoving = false;
            triggerable.OnPlayerTriggered(this);
            break;
        }
      }
    }
    public string Name
    {
        get => playerName;
    }

    public Sprite Sprite
    {
        get => sprite;
    }
}
