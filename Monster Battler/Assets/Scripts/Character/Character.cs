using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    CharacterAnimator characterAnimator;

    public bool IsMoving {get; set;}

    private void Awake() 
    {
        characterAnimator = GetComponent<CharacterAnimator>();

    }
    public IEnumerator Move(Vector2 moveDirection, Action OnMoveOver = null)
    {
        
        //Round up to nearest interger
        float horizontal = Mathf.Round(moveDirection.x);
        float vertical = Mathf.Round(moveDirection.y);

        if(Mathf.Abs(horizontal) > Mathf.Abs(vertical)) //prevent diagonal movement
        {
                vertical= 0;
                
        }
        else
        {
                horizontal = 0;
        }

        //Animations
        characterAnimator.MoveX = Mathf.Clamp(horizontal, -1f, 1f);
        characterAnimator.MoveY = Mathf.Clamp(vertical, -1f, 1f);

        //Target Positions for Movement
        var targetPosition = transform.position;
        targetPosition.x += horizontal;
        targetPosition.y += vertical;

        if(!IsWalkable(targetPosition))
        {
            yield break;
        }

        IsMoving = true;

        while( (targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon )
        {

            float delta = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, delta);
            yield return null;
        }

        transform.position = targetPosition;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        characterAnimator.IsMoving = IsMoving;
    }

    bool IsWalkable(Vector3 targetPosition)
    {
        if(Physics2D.OverlapCircle(targetPosition, 0.2f, GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }
        
            return true;
    }

    public CharacterAnimator CharacterAnimator
    {
        get => characterAnimator;

    }
}
