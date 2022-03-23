using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond = 30;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] GameObject targetSelector;

    [SerializeField] GameObject backButton;

    Button[] moveButtons; 
    Button[] targetButtons;
    
    void Awake() 
    {
      moveButtons  = moveSelector.GetComponentsInChildren<Button>(); //get our move buttons
      targetButtons = targetSelector.GetComponentsInChildren<Button>(); //get our target buttons
    }


    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray()) //loops through each letter in string 1 by 1
        {
            dialogText.text += letter;
            
            yield return new WaitForSeconds(1f/lettersPerSecond); //waits for a portion of a second
        }

            yield return new WaitForSeconds(1f);
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
        
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        EnableBackButton(enabled);
    }

    public void EnableMoveDetails(bool enabled)
    {
        moveDetails.SetActive(enabled);
    }

    public void EnableTargetSelector(bool enabled)
    {
        targetSelector.SetActive(enabled);
        EnableBackButton(enabled);
    }

    public void EnableBackButton(bool enabled)
    {
        backButton.SetActive(enabled);
    }

    public void SetMoveNames(List<Move> moves)
    {

        for (int i=0; i < moveButtons.Length; ++i)
        {
            if (i < moves.Count) //in the event we have less than 4 moves
            {
                moveButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = moves[i].Base.MoveName;
            }
            else
            {
                moveButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "-";
                //maybe change the color of the button too or test disabling the button? 
            }

        }
    }

    public void SetTargetNames(List<BattleUnit> units)
    {

        for (int i=0; i < targetButtons.Length; ++i)
        {
            if (i < units.Count) //in the event we have less than 4 units
            {
                targetButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = units[i].Monster.Base.MonsterName;
            }
            else
            {
                targetButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "-";
                //maybe change the color of the button too or test disabling the button? 
            }

        }
    }

    
}
