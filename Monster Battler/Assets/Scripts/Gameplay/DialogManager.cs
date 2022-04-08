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

    public event Action OnDialogStart;
    public event Action OnDialogEnd;

    Dialog dialog;
    int currentLine = 0;

    bool isTyping;

    PlayerControls dialogControls;

    private void Awake() 
    {
        Instance = this;

        dialogControls = new PlayerControls();
        dialogControls.Dialog.SetCallbacks(this);     
    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        
        OnDialogStart?.Invoke();

        this.dialog = dialog;
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
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                OnDialogEnd?.Invoke();
                dialogControls.Disable();
            }
        }
    }

    public void HandleUpdate()
    {
        if(Keyboard.current.eKey.wasPressedThisFrame && !isTyping)
        {
            //testing
        }

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
