using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    Vector2 moveInput;
    bool isMoving;
    
    
    
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
         if(!isMoving && moveInput != Vector2.zero)
        {
            if(moveInput.x !=0 ) moveInput.y = 0; //prefent horizontal movement

            var targetPosition = transform.position;
            targetPosition.x += moveInput.x;
            targetPosition.y += moveInput.y;

            StartCoroutine(Movement(targetPosition));
        }
        
    }

    IEnumerator Movement(Vector3 targetPosition)
    {
        isMoving = true;
        while( (targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon )
        {

            float delta = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, delta);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }


}
