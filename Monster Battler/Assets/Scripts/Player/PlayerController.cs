using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    Vector2 moveInput;
    bool isWalking;

    Animator playerAnimator;
    
    
    void Awake() 
    {
        playerAnimator = GetComponent<Animator>();    
    }
    void Update()
    {
        ProcessMovement();
    }

    void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();

        
        
    }

    void ProcessMovement()
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
            StartCoroutine(Movement(targetPosition));
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
    }


}
