using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    [SerializeField] LayerMask solidObjects;
    [SerializeField] LayerMask monsterEncounters;
    Vector2 moveInput;
    bool isWalking;

    Animator playerAnimator;

    public event Action OnEncounter;
    
    
    void Awake() 
    {
        playerAnimator = GetComponent<Animator>();    
    }
    void Update()
    {
        
    }

    void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();

        
        
    }

    public void HandleUpdate()
    {
        

         if(!isWalking && moveInput != Vector2.zero)
        {
            if(moveInput.x !=0 ) moveInput.y = 0; //prefent horizontal movement

            //Animations
            playerAnimator.SetFloat("moveX", moveInput.x);
            playerAnimator.SetFloat("moveY", moveInput.y);

            //Target Positions for Movement
            var targetPosition = transform.position;
            targetPosition.x += moveInput.x;
            targetPosition.y += moveInput.y;

            //Movement is handled by couroutine
            if(IsWalkable(targetPosition))
            {
                StartCoroutine(Movement(targetPosition));
            }
            
        }
        
        playerAnimator.SetBool("isWalking", isWalking);
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
        if(Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjects) != null)
        {
            return false;
        }
        
            return true;
    }

    void CheckForEncounter()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, monsterEncounters)!= null )
        {
            if(UnityEngine.Random.Range(1, 101) <= 10) //10% chance of random monster encounter
            {
                playerAnimator.SetBool("isWalking", false);
                OnEncounter();

            }
        }
    }


}
