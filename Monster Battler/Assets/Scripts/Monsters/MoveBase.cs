using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Monsters/New Move", order = 0)]
public class MoveBase : ScriptableObject
{
     [Header("General")]
    [SerializeField] string moveName;
    
    [TextArea]
    [SerializeField] string description;
    [SerializeField] MonsterType type;
    [SerializeField] bool isSpecial;

    [Header("Stats")]
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int ap;

    //Properties
    public string MoveName
    {
        get
        {
            return moveName;
        }
    }
    public string Description
    {
        get
        {
            return description;
        }
    
    }
    public MonsterType Type
    {
        get
        {
            return type;
        }
    }
    public bool IsSpecial
    {
        get
        {
            return isSpecial;
        }
        
    }
    public int Power
    {
        get
        {
            return power;
        }
    
    }
    public int Accuracy
    {
        get
        {
            return accuracy;
        }
    
    }
    public int Ap
    {
        get
        {
            return ap;
        }
    
    }

}
