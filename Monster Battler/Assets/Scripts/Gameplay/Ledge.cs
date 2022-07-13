using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Ledge : MonoBehaviour
{
    [SerializeField] int xDir;
    [SerializeField] int yDir;
    [SerializeField] float jumpPower = 0.3f;
    [SerializeField] float jumpDuration = 0.5f;

    private void Awake() 
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
    public bool AttemptToJump(Character character, Vector2 moveDirection)
    {
        
        if(moveDirection.x == xDir && moveDirection.y == yDir)
        {
            StartCoroutine(Jump(character));
            return true;
        }

        return false;
    }

    IEnumerator Jump(Character character)
    {
        GameController.Instance.PauseGame(true); //prefent player from moving while jumping

        Vector3 jumpDestination = character.transform.position + new Vector3 (xDir, yDir) * 2;
        character.CharacterAnimator.IsJumping = true;
        yield return character.transform.DOJump(jumpDestination, jumpPower, 1, jumpDuration).WaitForCompletion();
        
        character.CharacterAnimator.IsJumping = false;
        GameController.Instance.PauseGame(false);
         
    }
}
