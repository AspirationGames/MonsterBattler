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
    bool isConfirmed;

    PlayerControls dialogControls;

    private void Awake() 
    {
        Instance = this;

        dialogControls = new PlayerControls();
        dialogControls.Dialog.SetCallbacks(this);     
    }

    public IEnumerator ShowDialogText(string text, bool waitForInput = false, bool autoClose = true) //simple text messeage not dialogue if set to true the player will still need to give input to close dialogue
    {
        OnDialogStart?.Invoke();
        dialogControls.Enable();
        IsShowing = true;
        dialogBox.SetActive(true);
        

        yield return TypeDialog(text);
        
        if(waitForInput)
        {
            yield return new WaitUntil(() => isConfirmed);
        }

        yield return new WaitForSeconds(0.5f);


        if(autoClose == true)
        {
            CloseDialog();
        }
        OnDialogEnd?.Invoke();
        dialogControls.Disable();
        
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }
    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        
        OnDialogStart?.Invoke();
        dialogBox.SetActive(true);
        IsShowing = true;
        dialogControls.Enable();
        
        
        foreach(var line in dialog.Lines)
        {
            yield return TypeDialog(line);
            yield return new WaitUntil(() => isConfirmed);

        }
        
        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogEnd?.Invoke();
        dialogControls.Disable();
        
        //yield return TypeDialog(dialog.Lines[0]); //removing for now to test foreah loop
        
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        
        isConfirmed = context.performed;
        
    }

    public void HandleUpdate()
    {
        //don't think I need this anymore I was previously handling OnConfirm here. Keeping in place for now.
    }
    

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        foreach(var letter in line.ToCharArray()) //loops through each letter in string 1 by 1
        {
            dialogText.text += letter;
            
            yield return new WaitForSeconds(1f/lettersPerSecond); //waits for a portion of a second
        }
        
    }

    
}
