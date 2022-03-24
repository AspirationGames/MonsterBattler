using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    [SerializeField] MonsterBase _base;
    [SerializeField] int level;

    [SerializeField] Personality personality;

    const float personalityStatIncrease = 1.10f;
    const float personalityStatDecrease = 0.90f;

    [Header("IVs")]
    [SerializeField][Range(0, 31)] int naturalSkillMaxHP; //IVs
    [SerializeField][Range(0, 31)] int naturalSkillAttack; //IVs
    [SerializeField][Range(0, 31)] int naturalSkillDefense; //IVs
    [SerializeField][Range(0, 31)] int naturalSkillSpAttack; //IVs
    [SerializeField][Range(0, 31)] int naturalSkillSpDefense; //IVs
    [SerializeField][Range(0, 31)] int naturalSkillSpeed; //IVs

    [Header("EVs")]
    [SerializeField][Range(0, 252)] int developedSkillMaxHP; //EVs
    [SerializeField][Range(0, 252)] int developedSkillAttack; //EVs
    [SerializeField][Range(0, 252)] int developedSkillDefense; //EVs
    [SerializeField][Range(0, 252)] int developedSkillSpAttack; //EVs
    [SerializeField][Range(0, 252)] int developedSkillSpDefense; //EVs
    [SerializeField][Range(0, 252)] int developedSkillSpeed; //EVs
    public MonsterBase Base 
    {
        get
        {
            return _base;
        }
    } //set to property so we can refrence the base from the Pokemon Class directily in other classes (i.e. battlehud script);
    public int Level 
    {
        get
        {
            return level;
        }
    } //set to property so we can refrence the base from the Pokemon Class directily in other classes (i.e. battlehud script);

    public int HP {get; set;} //our monsters current HP, we are using a property
    public List<Move> Moves{ get; set;} //we are using a property for the moves
    public Dictionary<Stat, int> Stats {get; private set;} //we can get stats publically but only set stats in the monster class
    public Dictionary<Stat, int> StatStages {get; private set;} //integer values in this dictionary are between minus 6 and plus 6
    public Queue<string> StatusChangeMessages {get; private set;} = new Queue<string>();
    
    public Condition Status{get; private set;}
    public bool InBattle {get; set;} //flag for if monster is actively in battle
    public bool HpChanged {get; set;}


    public void Init() //this method creates our pokemon
    {
        //Generate Moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.MoveLevel <= Level) //add moves if the pokemons level is above the move level
            {
                Moves.Add(new Move(move.Base));
            }
            if(Moves.Count >= 4) // break our loop if the pokemon already has 4 moves
            {
                break;
            }
        }

        CalculateStats();
        HP = MaxHP;

        ResetStatStages();
        
    }

    float PersonalityValue(Stat stat)
    {   //TO DO ADD FULL LIST OF PERSONALITY VALUES
        switch(stat)
        {
            case Stat.Attack:
                switch(personality)
                {
                    case Personality.Lonely:
                    case Personality.Brave:
                    case Personality.Adamant:
                    case Personality.Naughty:
                        return personalityStatIncrease;
                    case Personality.Bold:
                    case Personality.Timid:
                    case Personality.Modest:
                    case Personality.Calm:
                        return personalityStatDecrease;
                }
                break;
            case Stat.Defense:
                switch(personality)
                {
                    case Personality.Bold:
                    case Personality.Relaxed:
                    case Personality.Impish:
                    case Personality.Lax:
                        return personalityStatIncrease;
                    case Personality.Lonely:
                    case Personality.Hasty:
                    case Personality.Mild:
                    case Personality.Gentle:
                        return personalityStatDecrease;
                }
                break;
            case Stat.SpAttack:
                switch(personality)
                {
                    case Personality.Modest:
                    case Personality.Mild:
                    case Personality.Quiet:
                    case Personality.Rash:
                        return personalityStatIncrease;
                    case Personality.Adamant:
                    case Personality.Impish:
                    case Personality.Jolly:
                    case Personality.Careful:
                        return personalityStatDecrease;
                }
                break;
            case Stat.SpDefense:
                switch(personality)
                {
                    case Personality.Calm:
                    case Personality.Gentle:
                    case Personality.Sassy:
                    case Personality.Careful:
                        return personalityStatIncrease;
                    case Personality.Naughty:
                    case Personality.Lax:
                    case Personality.Naive:
                    case Personality.Rash:
                        return personalityStatDecrease;
                }
                break;
            case Stat.Speed:
                switch(personality)
                {
                    case Personality.Timid:
                    case Personality.Hasty:
                    case Personality.Jolly:
                    case Personality.Naive:
                        return personalityStatIncrease;
                    case Personality.Brave:
                    case Personality.Relaxed:
                    case Personality.Quiet:
                    case Personality.Sassy:
                        return personalityStatDecrease;
                }
                break;
            default:
                return 1f;
        }

        return 1f;  //for some stupid reason we need this here even though the switch should always return a value
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();

        //HP has unique formula and is not effected by personality
        MaxHP = Mathf.FloorToInt
            (
                ( ( (2*Base.MaxHP + naturalSkillMaxHP + developedSkillMaxHP/4)* Level / 100  ) + Level + 10 ) 
            ); 

        //OtherStats

        Stats.Add
            (Stat.Attack, 
                Mathf.FloorToInt
                ( 
                    ( ( (2*Base.Attack + naturalSkillAttack + developedSkillAttack/4)* Level / 100  ) + 5 ) 
                    * PersonalityValue(Stat.Attack)
                )
        );
        Stats.Add
            (Stat.Defense, 
                Mathf.FloorToInt
                ( 
                    ( ( (2*Base.Defense + naturalSkillDefense + developedSkillDefense/4)* Level / 100  ) + 5 ) 
                    * PersonalityValue(Stat.Defense)
                )
        );
        Stats.Add
            (Stat.SpAttack, 
                Mathf.FloorToInt
                ( 
                    ( ( (2*Base.SpAttack + naturalSkillSpAttack + developedSkillSpAttack/4)* Level / 100  ) + 5 ) 
                    * PersonalityValue(Stat.SpAttack)
                )
        );
        Stats.Add
            (Stat.SpDefense, 
                Mathf.FloorToInt
                ( 
                    ( ( (2*Base.SpDefense + naturalSkillSpDefense + developedSkillSpDefense/4)* Level / 100  ) + 5 ) 
                    * PersonalityValue(Stat.SpDefense)
                )
        );
        Stats.Add
            (Stat.Speed, 
                Mathf.FloorToInt
                ( 
                    ( ( (2*Base.Speed + naturalSkillSpeed + developedSkillSpeed/4)* Level / 100  ) + 5 ) 
                    * PersonalityValue(Stat.Speed)
                )
        );

    }

    void ResetStatStages()
    {
        StatStages = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0}
        };
    }

    int GetStat(Stat stat)
    {
       int statValue = Stats[stat];

       int statStage = StatStages[stat];
       var stageModifiers = new float[] {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f}; //note that decreases are divided and boost are multiplies)

       if(statStage >= 0)
       {
            statValue = Mathf.FloorToInt(statValue * stageModifiers[statStage]);    
       }
       else if(statStage <0 )
       {
           statValue = Mathf.FloorToInt(statValue / stageModifiers[-statStage]);//stat stage is negative so we negate in order to have valid index
       }

       return statValue; 
    }

    public void ApplyStageChange(List<StatStageChange> stageChanges)
    {
        foreach(StatStageChange stageChange in stageChanges)
        {
           var stat = stageChange.stat;
           var stage = stageChange.stage;

           StatStages[stat] = Mathf.Clamp(StatStages[stat] + stage, -6, 6); //applies stage change to current StatStages

           if(stage > 0) StatusChangeMessages.Enqueue($"{Base.MonsterName}'s {stat} increased!");
           else if(stage < 0) StatusChangeMessages.Enqueue($"{Base.MonsterName}'s {stat} fell!");


        }
        
    }

    public void SetStatus(ConditionID conditionID)
    {
        Status = ConditionsDB.Conditions[conditionID];
        StatusChangeMessages.Enqueue($"{Base.MonsterName} {Status.StartMessage}");
    }

    public bool OnBeforeMove()
    {
       if(Status?.OnBeforeMove != null)
        {
            return Status.OnBeforeMove(this); //if the pokemon has a status the OnBeforeMove method will run and deterime if mon can move
        }

        return true;
    }
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this); //addomg a question mark after action will make sure that on after turn is not null
    }

    

    //Stat Properties
    
    public int MaxHP {get; private set;}
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }
    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    } 
    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }
    
    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        //critical hits
        float critical = 1f;
        if(Random.value * 100f <= 6.25f) critical = 2f;

        //type effectiveness
        float typeEffectiveness = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        //STAB bonus
        float stab = 1f;
        if(this.Base.Type1 == move.Base.Type || this.Base.Type2 == move.Base.Type) stab = 1.5f;

        var damageDetails = new DamageDetails()
        {
            Critical = critical,
            TypeEffectiveness = typeEffectiveness
            
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack; //checks to see if move is special to determine attack type. This is a shorthand if statement
        float defense = (move.Base.Category == MoveCategory.Special)? SpDefense : Defense;

        float modifiers = Random.Range(0.85f, 1f) * typeEffectiveness * critical * stab;
        float a = (2*attacker.Level + 10)/ 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d*modifiers);

        

        UpdateHP(damage);
        
        return damageDetails;

        
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HpChanged = true;
    }

    public void OnBattleOver() //used to reset values after battle is over
    {
        ResetStatStages();
    }
}

public class DamageDetails
{
    public bool Fainted{get; set;}
    public float Critical{get; set;}
    public float TypeEffectiveness{get; set;}
}
