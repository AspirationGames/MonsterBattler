using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChoiceBox : MonoBehaviour
{
    
    [SerializeField] GameObject choiceButtonPrefab;
    bool isChoiceSelected = false;

    int selectedChoiceIndex;

    private void Start() 
    {
        ChoiceButton.choiceSelected += OnChoiceSelected; 
    }
    public IEnumerator ShowChoices(List<string> choices, Action<int> choiceAction)
    {

        isChoiceSelected = false;
        gameObject.SetActive(true);

        //delete existing choices
        foreach(ChoiceButton child in GetComponentsInChildren<ChoiceButton>())
        {
            Destroy(child.gameObject);
        }

        //Set new choices
        foreach(string choice in choices)
        {
            var choiceButton = Instantiate(choiceButtonPrefab, transform); //instantiates choice buttons as chidlren;
            var choiceButtonText = choiceButton.GetComponent<ChoiceButton>().TextUI;
            choiceButtonText.text = choice;
        }

        yield return new WaitUntil(() => isChoiceSelected == true);

        choiceAction?.Invoke(selectedChoiceIndex);
        gameObject.SetActive(false);
    }

    public void OnChoiceSelected(ChoiceButton choice)
    {
        selectedChoiceIndex = choice.transform.GetSiblingIndex();
        isChoiceSelected = true;
    }
}
