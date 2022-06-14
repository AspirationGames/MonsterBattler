using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopQtyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] TextMeshProUGUI priceText;

    bool qtyConfirmed;
    int currentQty;
    int MaxCount;
    int pricePerUnit;
    public IEnumerator ShowQuantity(int MaxQty, int pricePerUnit, Action<int> onQtyConfirmed)
    {
        
        this.MaxCount = MaxQty;
        this.pricePerUnit = pricePerUnit;
        
        qtyConfirmed = false;
        currentQty = 1;


        gameObject.SetActive(true);
        SetValues();

        yield return new WaitUntil(() => qtyConfirmed == true);

        onQtyConfirmed?.Invoke(currentQty);
        gameObject.SetActive(false);

    }

    public void IncreaseQuantity()
    {
        currentQty++;
        SetValues();
    }

    public void DecreaseQuantity()
    {
        currentQty--;
        SetValues();
    }

    public void SetValues()
    {
        currentQty = Mathf.Clamp(currentQty, 1, MaxCount);

        countText.text = $"x {currentQty}";
        priceText.text = $"x {currentQty * pricePerUnit}";
    }

    public void ConfirmSelection()
    {
        qtyConfirmed = true;
    }
}
