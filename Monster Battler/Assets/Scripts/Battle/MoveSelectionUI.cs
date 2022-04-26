using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveSelectionUI : MonoBehaviour
{
    
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] Image monsterImage;

    public void SetMoveData(List <MoveBase> currentMoves, MoveBase newMove)
    {

        for (int i= 0; i<currentMoves.Count; ++i ) //set move names
        {
            moveTexts[i].text = currentMoves[i].MoveName;
        }

        moveTexts[currentMoves.Count].text = newMove.MoveName;
    }

    public void SetMonsterImage(Monster monster)
    {
        monsterImage.sprite = monster.Base.FrontSprite;

    }


}
