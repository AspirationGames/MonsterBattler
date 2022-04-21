using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerMonster;
    [SerializeField] BattleHud hud;

    [SerializeField] float enterAnimationDuration = 1f;

    //General Properties
    public BattleHud Hud 
    {
        get { return hud;}
    }
    public bool IsPlayerMonster
    {
        get { return isPlayerMonster; }
    }

    public Monster Monster {get; set;} 

    //Animation Variables
    Image monsterImage;
    Vector3 startPosition;

    Color originalColor;
    private void Awake() 
    {
        monsterImage = GetComponent<Image>();
        startPosition = monsterImage.transform.localPosition;   
        originalColor = monsterImage.color; 
    }
    public void Setup(Monster monster)
    {
       Monster =  monster;

       if(isPlayerMonster)
       {
           monsterImage.sprite = Monster.Base.BackSprite;
       }
       else
       {
           monsterImage.sprite = Monster.Base.FrontSprite;
       }

        //Set Hud
        hud.gameObject.SetActive(true);
        hud.SetData(monster);

        //Reset Unit Animations
        transform.localScale = new Vector3(1, 1, 1);
        monsterImage.color = originalColor;
        
        //Enter Animations
        PlayEnterAnimation();

    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        if(isPlayerMonster)
        {
            monsterImage.transform.localPosition = new Vector3(-2000f, startPosition.y);
        }
        else
        {
            monsterImage.transform.localPosition = new Vector3(2000f, startPosition.y);
        }

        monsterImage.transform.DOLocalMoveX(startPosition.x, enterAnimationDuration);
        
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if(isPlayerMonster)
        {
           sequence.Append(monsterImage.transform.DOLocalMoveX(startPosition.x + 50f, 0.25f));
        }
        else
        {
            sequence.Append(monsterImage.transform.DOLocalMoveX(startPosition.x + -50f, 0.25f));
        }

        sequence.Append(monsterImage.transform.DOLocalMoveX(startPosition.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        
        var sequence = DOTween.Sequence();
        sequence.Append(monsterImage.DOColor(Color.gray, 0.1f));
        sequence.Append(monsterImage.DOColor(originalColor, 0.1f));

    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(monsterImage.transform.DOLocalMoveY(startPosition.y - 500f, 0.5f ));
        sequence.Join(monsterImage.DOFade(0f, 0.5f));
    }

    public IEnumerator PlayBindingAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(monsterImage.DOFade(0,0.5f));
        sequence.Join(transform.DOLocalMoveY(startPosition.y - 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f,0.3f,1f), 0.5f));
        
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(monsterImage.DOFade(1,0.5f));
        sequence.Join(transform.DOLocalMoveY(startPosition.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f,1f,1f), 0.5f));
        
        yield return sequence.WaitForCompletion();
    }

}
