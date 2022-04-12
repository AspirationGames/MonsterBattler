using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogManager : MonoBehaviour, PlayerControls.IDialogActions
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;

    [SerializeField] int lettersPerSecond;

    public static DialogManager Instance {get; private set;}
    public bool IsShowing {get; private set;}

    public event Action OnDialogStart;
    public event Action OnDialogEnd;

    Dialog dialog;
    Action onDialogFinished;
    int currentLine = 0;

    bool isTyping;

    PlayerControls dialogControls;

    private void Awake() 
    {
        Instance = this;

        dialogControls = new PlayerControls();
        dialogControls.Dialog.SetCallbacks(this);     
    }

    public IEnumerator ShowDialog(Dialog dialog, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame();
        
        OnDialogStart?.Invoke();

        IsShowing = true;
        this.dialog = dialog;
        onDialogFinished = onFinished;
        dialogBox.SetActive(true);
        dialogControls.Enable();
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public bool IsInDialogState()
    {
        if(GameController.Instance.GameState == GameState.Dialog)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        
        if(!isTyping && context.performed && IsInDialogState())
        {
            ++currentLine;
            if(currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else //close dialog
            {
                currentLine = 0;
                IsShowing = false;
                dialogBox.SetActive(false);
                onDialogFinished?.Invoke(); //NPC action
                OnDialogEnd?.Invoke(); //Player Action
                dialogControls.Disable();
            }
        }
    }

    public void HandleUpdate()
    {
        //don't think I need this anymore I was previously handling OnConfirm here. Keeping in place for now.
    }
    

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;

        dialogText.text = "";
        foreach(var letter in line.ToCharArray()) //loops through each letter in string 1 by 1
        {
            dialogText.text += letter;
            
            yield return new WaitForSeconds(1f/lettersPerSecond); //waits for a portion of a second
        }

        isTyping = false;
        
    }

    
}
