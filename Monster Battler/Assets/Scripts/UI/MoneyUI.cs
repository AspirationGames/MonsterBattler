using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;

    private void Awake() 
    {
          
    }

    private void Start() 
    {
        PlayerMoney.i.OnMoneyChanged += SetMoneyText;  
        SetMoneyText();  
    }

    public void ShowMoneyBox()
    {
        gameObject.SetActive(true);
    }

    public void CloseMoneyBox()
    {
        gameObject.SetActive(false);
    }
    public void SetMoneyText()
    {
        moneyText.text = $"Money: {PlayerMoney.i.Money}";
    }



}
