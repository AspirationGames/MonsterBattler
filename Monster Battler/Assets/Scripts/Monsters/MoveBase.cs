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

    [Header("Stats")]
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int ap;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;

    

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

    public bool AlwaysHits
    {
        get
        {
            return alwaysHits;
        }
    
    }
    public int Ap
    {
        get
        {
            return ap;
        }
    
    }
    public MoveCategory Category
    {
        get
        {
            return category;
        }
    }

    public MoveEffects Effects
    {
        get
        {
            return effects;
        }
    }

    public MoveTarget Target 
    {
        get {return target;}
    }

}

[System.Serializable]
    public class MoveEffects
    {
        [SerializeField] List<StatStageChange> stageChange;
        [SerializeField] ConditionID statusEffect;
        [SerializeField] ConditionID volatileStatusEffect;

        public List<StatStageChange> StageChanges
        {
            get{return stageChange;}
        }

        public ConditionID StatusEffect
        {
            get{return statusEffect;}
        }

        public ConditionID VolatileStatusEffect
        {
            get{return volatileStatusEffect;}
        }

    }

    [System.Serializable]
    public class StatStageChange
    {
        public Stat stat;
        public int stage;
    }

    public enum MoveCategory
    {
        Physical,
        Special,
        Status

    }

    public enum MoveTarget
    {
        Enemy, Self
    }
