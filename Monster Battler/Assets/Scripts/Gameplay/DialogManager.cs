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

    public IEnumerator ShowDialogText(string text, bool waitForInput = false, bool autoClose = true) //simple text messeage not dialogue if set to true the player will still need to give input to close dialogue
    {
        IsShowing = true;
        dialogBox.SetActive(true);

        yield return TypeDialog(text);
        
        if(waitForInput)
        {
            //This currently defualts to false
            //we can require confirmaiton input here, but need to figure out how to implement with new inptu ssytem instead will just wait an extra second before closing dialgoue for now
        }

        yield return new WaitForSeconds(0.5f);


        if(autoClose == true)
        {
            CloseDialog();
        }
        
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
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

    public void OnConfirm(InputAction.CallbackContext context)
    {
        
        if(!isTyping && context.performed && GameController.Instance.GameState == GameState.Dialog)
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
