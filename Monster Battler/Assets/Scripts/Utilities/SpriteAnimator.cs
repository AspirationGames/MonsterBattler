using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    //this script will create custome animations for 2D sprites by cyling through the sprites in the sprite rendered

    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate; //interval at which we change the sprites

    int currentFrame;
    float timer;

    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate = 0.16f)
    {
        this.frames = frames;
        this.spriteRenderer = spriteRenderer;
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0;
        spriteRenderer.sprite = frames[0];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if(timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }

    public List<Sprite> Frames
    {
        get {return frames;}

        
    }
}
