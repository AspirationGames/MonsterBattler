using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions 
{
    
    [SerializeField] float moveSpeed = 5f;

    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask monsterEncountersLayer;

    [SerializeField] float encoutnerRate = 10f;
    bool isWalking;

    Animator playerAnimator;

    Vector2 moveDirection;
    public event Action OnEncounter;
    
    PlayerControls playerControls;
    void Awake() 
    {
        playerAnimator = GetComponent<Animator>();  

        playerControls = new PlayerControls();
        playerControls.Player.SetCallbacks(this);  

        Debug.Log(playerControls != null);
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
            return;
        
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

         if(!isWalking && moveDirection != Vector2.zero)
        {
            if(Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y)) //prevent horizontal movement
            {
                moveDirection.y = 0;
                
            }
            else
            {
                moveDirection.x = 0;
            }


            //Animations
            playerAnimator.SetFloat("moveX", moveDirection.x);
            playerAnimator.SetFloat("moveY", moveDirection.y);

            //Target Positions for Movement
            var targetPosition = transform.position;
            targetPosition.x += Mathf.Round(moveDirection.x);
            targetPosition.y += Mathf.Round(moveDirection.y);

            //Movement is handled by couroutine
            if(IsWalkable(targetPosition))
            {
                StartCoroutine(Movement(targetPosition));
            }
            
        }

        
        
        
        playerAnimator.SetBool("isWalking", isWalking);
    }

    void Interact()
    {
        var faceDirection = new Vector3(playerAnimator.GetFloat("moveX"), playerAnimator.GetFloat("moveY"));
        var interactPosition = transform.position + faceDirection;

        //Debug.DrawLine(transform.position, interactPosition, Color.green, 1f);

        var interactableCollider = Physics2D.OverlapCircle(interactPosition, 0.3f, interactableLayer);
        
        if (interactableCollider != null)
        {
            interactableCollider.GetComponent<Interactable>()?.Interact(); //finds Interactable interace. Any interactable items should have this interface attached to their class
        }

    }

    IEnumerator Movement(Vector3 targetPosition)
    {
        isWalking = true;
        while( (targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon )
        {

            float delta = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, delta);
            yield return null;
        }

        transform.position = targetPosition;
        isWalking = false;

        CheckForEncounter();
    }

    bool IsWalkable(Vector3 targetPosition)
    {
        if(Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }
        
            return true;
    }

    void CheckForEncounter()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, monsterEncountersLayer)!= null )
        {
            if(UnityEngine.Random.Range(1, 101) <= encoutnerRate) //10% chance of random monster encounter
            {
                playerAnimator.SetBool("isWalking", false);
                OnEncounter();

            }
        }
    }

    
}
