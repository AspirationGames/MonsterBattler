using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownFrames;
    [SerializeField] List<Sprite> walkUpFrames;
    [SerializeField] List<Sprite> walkRightFrames;
    [SerializeField] List<Sprite> walkLeftFrames;

   //Parameters
   public float MoveX {get; set;}
   public float MoveY {get; set;}
   public bool IsMoving {get; set;}

   //States

   SpriteAnimator walkDownAnim;
   SpriteAnimator walkUpAnim;
   SpriteAnimator walkRightAnim;
   SpriteAnimator walkLeftAnim;

   SpriteAnimator currentAnim;
   bool wasPreviouslyMoving;

    //Reference
    SpriteRenderer spriteRenderer;

    void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
           
    }
    private void Start() 
    {
        walkDownAnim = new SpriteAnimator (walkDownFrames, spriteRenderer); 
        walkUpAnim = new SpriteAnimator (walkUpFrames, spriteRenderer);
        walkRightAnim = new SpriteAnimator (walkRightFrames, spriteRenderer);
        walkLeftAnim = new SpriteAnimator (walkLeftFrames, spriteRenderer);

        currentAnim = walkDownAnim;
    }

    private void Update() 
    {
        var previousAnim = currentAnim;
        //Set Animations based on move input
        if(MoveY == -1)
        {
            currentAnim = walkDownAnim;
        }
        else if(MoveY == 1)
        {
            currentAnim = walkUpAnim;
        }
        else if(MoveX == 1)
        {
            currentAnim = walkRightAnim;
        }   
        else if(MoveX == -1)
        {
            currentAnim = walkLeftAnim;
        }

        if(currentAnim != previousAnim || IsMoving != wasPreviouslyMoving) //if the player switch direction we need to reset the animation
        {
            currentAnim.Start();
        }

        //play animations when moving
        if(IsMoving)
        {
            currentAnim.HandleUpdate();
        }
        else //play first frame aka idle
        {
            spriteRenderer.sprite = currentAnim.Frames[0];
        }

        wasPreviouslyMoving = IsMoving;
    }


}
