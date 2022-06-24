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

    [SerializeField] MoveTarget target;

    [Header("Stats")]
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int ap;
    [SerializeField] int priority;

    [Header("Effects")]
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects; //moves can have multiple secondary effects
    
    [Header("Audio")]
    [SerializeField] AudioClip moveSFX;

    

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
    public int AP
    {
        get
        {
            return ap;
        }
    
    }

    public int Priority
    {
        get
        {
            return priority;
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

    public List<SecondaryEffects> SecondaryEffects
    {
        get
        {
            return secondaryEffects;
        }
    }

    public MoveTarget Target 
    {
        get {return target;}
    }

    public AudioClip MoveSFX => moveSFX;

}

[System.Serializable]
    public class MoveEffects
    {
        [SerializeField] List<StatStageChange> stageChange;
        [SerializeField] ConditionID statusEffect;
        [SerializeField] ConditionID volatileStatusEffect;
        [SerializeField] ConditionID weather;
        [SerializeField] ConditionID timeWarp;
        [SerializeField] ConditionID protect;


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

        public ConditionID Weather
        {
            get{return weather;}
        }

        public ConditionID TimeWarp
        {
            get{return timeWarp;}
        }

        public ConditionID Protect
        {
            get{return protect;}
        }

    }
[System.Serializable]
    public class SecondaryEffects : MoveEffects //inherits attributes from MoveEffects class
    {
        [SerializeField] int chance;
        [SerializeField] MoveTarget target;

        public int Chance
        {
            get { return chance;}
        }
        public MoveTarget Target
        {
            get { return target;}
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
