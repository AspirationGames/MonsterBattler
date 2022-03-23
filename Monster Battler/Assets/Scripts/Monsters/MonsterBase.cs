using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monsters/New Monster", order = 0)]
public class MonsterBase : ScriptableObject 
{
    [Header("General")]
    [SerializeField] string monsterName;
    
    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] MonsterType type1;
    [SerializeField] MonsterType type2;

    [Header("Stats")]
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [Header("MovePool")]
    [SerializeField] List<LearnableMoves> learnableMoves;

    //Propterties
    public string MonsterName
    {
        get
        {
            return monsterName;
        }
    
    }
    public string Description
    {
        get
        {
            return description;
        }
    
    }
    public Sprite FrontSprite
    {
        get
        {
            return frontSprite;
        }
    
    }
    public Sprite BackSprite
    {
        get
        {
            return backSprite;
        }
    
    }
    public MonsterType Type1
    {
        get
        {
            return type1;
        }
    
    }
    public MonsterType Type2
    {
        get
        {
            return type2;
        }
    
    }
    public int MaxHP
    {
        get
        {
            return maxHP;
        }
    
    }
    public int Attack
    {
        get
        {
            return attack;
        }
    
    }
    public int Defense
    {
        get
        {
            return defense;
        }
    
    }
    public int SpAttack
    {
        get
        {
            return spAttack;
        }
    
    }
    public int SpDefense
    {
        get
        {
            return spDefense;
        }
    
    }
    public int Speed
    {
        get
        {
            return speed;
        }
    
    }

    public List<LearnableMoves> LearnableMoves
    {
        get{return learnableMoves;}
    }


}    

[System.Serializable]
public class LearnableMoves
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int moveLevel;

    //Properties
    public MoveBase Base
    {
        get{return moveBase;}
    }
    public int MoveLevel
    {
        get{return moveLevel;}
    }
}

public enum MonsterType 
{
    None,
    Normal,
    Grass,
    Fire,
    Water,
    Shadow,
    Necro,
    Solar,
    Bug,
    Flying,

}

public enum Stat
{
    HP,
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed
}

public enum Personality
{
    Hardy,
    Lonely,
    Brave,
    Adamant,
    Naughty,
    Bold,
    Docile,
    Relaxed,
    Impish,
    Lax,
    Timid,
    Hasty,
    Serious,
    Jolly,
    Naive,
    Modest,
    Mild,
    Quiet,
    Bashful,
    Rash,
    Calm,
    Gentle,
    Sassy,
    Careful,
    Quirky

}

public class TypeChart
{
    static float[][] chart = 
    {    //                 NOR GRS FIR WAT SHD NEC SOL BUG FLY
        /*NOR*/ new float[]{1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f},
        /*GRS*/ new float[]{1f,.5f,.5f, 2f, 1f, 1f, 1f,.5f,.5f},
        /*FIR*/ new float[]{1f, 2f,.5f,.5f, 1f, 1f, 1f, 2f, 1f},
        /*WAT*/ new float[]{1f,.5f, 2f,.5f, 1f, 1f, 1f, 1f, 1f},
        /*SHD*/ new float[]{1f, 1f, 2f, 1f,.5f, 2f, 2f, 1f, 1f},
        /*NEC*/ new float[]{1f, 1f, 1f, 1f,.5f,.5f,.5f, 1f, 1f},
        /*SOL*/ new float[]{1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 1f},
        /*BUG*/ new float[]{1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f},
        /*FLY*/ new float[]{1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f},
        

    };

    public static float GetEffectiveness(MonsterType attackType, MonsterType defenseType)
    {
        if (attackType == MonsterType.None || defenseType == MonsterType.None)
            return 1;
        
        int row = (int)attackType - 1; //our table index is the index of the enum monster type subtracted by 1. i.e. enum for normal is 1 so if we subtract 1 we get 0 which is the index of normal in type chart
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}

