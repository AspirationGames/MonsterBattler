using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoney : MonoBehaviour, ISavable
{
    [SerializeField] int money;

    public int Money => money;

    public event Action OnMoneyChanged;
    public static PlayerMoney i {get; private set;}

    private void Awake() 
    {
        i = this;    
    }
    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke();
    } 

    public void ReduceMoney(int amount)
    {
        money -= amount;
        OnMoneyChanged?.Invoke();
    }

    public object CaptureState()
    {
        return money;
    }

    public void RestoreState(object state)
    {
       money = (int)state;
    }
}
