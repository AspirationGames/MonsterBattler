using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    TextMeshProUGUI textUI;

    public static event Action<ChoiceButton> choiceSelected;

    public void Awake() 
    {
        textUI = GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(textUI.text);
    }

    public void OnChoiceSelected()
    {
        choiceSelected?.Invoke(this);
    }

    public TextMeshProUGUI TextUI => textUI;
}
