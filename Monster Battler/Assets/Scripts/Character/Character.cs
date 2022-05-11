using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    CharacterAnimator characterAnimator;

    public bool IsMoving {get; set;}

    public float OffSetY {get; private set;} = 0.3f;
    private void Awake() 
    {
        characterAnimator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);

    }

    public void SetPositionAndSnapToTile(Vector2 position)
    {
        position.x = MathF.Floor(position.x) + 0.5f;
        position.y = Mathf.Floor(position.y) + 0.5f + OffSetY;

        transform.position = position;
    }
    public IEnumerator Move(Vector2 moveDirection, Action RunAfterMove = null)
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

        if(!IsPathClear(targetPosition))
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

        RunAfterMove?.Invoke();
    }

    public void HandleUpdate()
    {
        characterAnimator.IsMoving = IsMoving;

        
    }

    bool IsPathClear(Vector3 targetPosition)
    {
        
        var diff = targetPosition - transform.position;
        var direction = diff.normalized;

        //Debug.DrawRay(transform.position + direction, direction, Color.green, 2f);

       if( Physics2D.BoxCast(transform.position + direction, new Vector2(0.2f,0.2f), 0f, direction, diff.magnitude - 1, 
            GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer) )
       {
           return false;
       }

        return true;  

    }

    bool IsWalkable(Vector3 targetPosition)
    {
        if(Physics2D.OverlapCircle(targetPosition, 0.2f, GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }
        
        return true;
    }

    public void LookTowards(Vector3 targetPosition)
    {
        var xDiff = Mathf.Floor(targetPosition.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPosition.y) - Mathf.Floor(transform.position.y);

        if(xDiff == 0 || yDiff == 0)
        {
            characterAnimator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            characterAnimator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
        }
        else
        {
            Debug.Log("Diagonal position.");
        }

    }

    public CharacterAnimator CharacterAnimator
    {
        get => characterAnimator;

    }
}
