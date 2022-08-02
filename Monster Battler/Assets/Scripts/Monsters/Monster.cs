using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public Monster(MonsterBase mBase, int mLevel)
    {
        _base = mBase;
        level = mLevel;

        Init();
    }
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
    public int Exp { get; set;}
    public ItemBase HeldItem {get; set;}
    public List<Move> Moves{ get; set;} //we are using a property for the moves
    public Dictionary<Stat, int> Stats {get; private set;} //we can get stats publically but only set stats in the monster class
    public Dictionary<Stat, int> StatStages {get; private set;} //integer values in this dictionary are between minus 6 and plus 6
    public Queue<string> StatusChangeMessages {get; private set;}
    
    public Condition Status{get; private set;}
    public int StatusTime{get; set;}

    public Condition VolatileStatus{get; private set;}
    public int VolatileStatusTime{get; set;}

    public Condition ProtectedStatus {get; set;} //protected
    public float ProtectSucessChance {get; set;}
    public bool IsProtected{get; set;}
    public bool InBattle {get; set;} //flag for if monster is actively in battle
    public event System.Action OnStatusChanged; 
    public event System.Action OnHPChanged; 
    public event System.Action OnLevelChanged;
    public event System.Action OnHeldItemChanged;
    


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
            if(Moves.Count >= MonsterBase.MaxNumberOfMoves) // break our loop if the pokemon already has 4 moves
            {
                break;
            }
        }

        Exp = Base.GetExpForLevel(level);

        CalculateStats();
        HP = MaxHP;

        StatusChangeMessages = new Queue<string>();
        ResetStatStages();
        ResetMoveLocks();
        Status = null;
        VolatileStatus = null;
        ProtectedStatus = null;
        ProtectSucessChance = 100f;
        
    }

    public Monster(MonsterSaveData saveData) //Loads the save data
    {
        _base = MonsterDB.GetObjectByName(saveData.name);
        HP = saveData.sHp;
        level = saveData.sLevel;
        Exp = saveData.sExp;
        personality = saveData.sPersonality;

        naturalSkillMaxHP = saveData.sNaturalSkillMaxHP;
        naturalSkillAttack = saveData.sNaturalSkillAttack;
        naturalSkillDefense = saveData.sNaturalSkillDefense;
        naturalSkillSpAttack = saveData.sNaturalSkillSpAttack;
        naturalSkillSpDefense = saveData.sNaturalSkillSpDefense;
        naturalSkillSpeed = saveData.sNaturalSkillSpeed;
        developedSkillMaxHP = saveData.sDevelopedSkillMaxHP;
        developedSkillAttack = saveData.sDevelopedSkillAttack;
        developedSkillDefense = saveData.sDevelopedSkillDefense;
        developedSkillSpAttack = saveData.sDevelopedSkillSpAttack;
        developedSkillSpDefense = saveData.sDevelopedSkillSpDefense;
        developedSkillSpeed = saveData.sDevelopedSkillSpeed;

        //Status
        if(saveData.sStatusId != null)
        {
            Status = ConditionsDB.Conditions[saveData.sStatusId.Value]; //.Value is used here because sStatudId is nullable
        }
        else
        {
            Status = null;
        }

        //Held Item
        if(saveData.sHeldItemName != null)
        {
            HeldItem = ItemDB.GetObjectByName(saveData.sHeldItemName);
            
        }
        else
        {
            HeldItem = null;
        }


        //Moves
        Moves = saveData.sMoves.Select(s => new Move(s)).ToList();


        //Reinitialize monster

        CalculateStats();
        StatusChangeMessages = new Queue<string>();
        ResetStatStages();
        VolatileStatus = null;
        ProtectedStatus = null;
        ProtectSucessChance = 100f;


    }
    public MonsterSaveData GetSaveData() //converst monster data into savable data
    {
        var saveData = new MonsterSaveData()
        {
            name = Base.name, //this is the scriptable object name
            sHp = HP,
            sLevel = Level,
            sExp = Exp,
            sHeldItemName = HeldItem?.name,
            sStatusId = Status?.Id,
            sPersonality = personality,
            sMoves = Moves.Select(m => m.GetMoveSaveData()).ToList(),
            sNaturalSkillMaxHP = naturalSkillMaxHP, //IVs
            sNaturalSkillAttack = naturalSkillAttack, //IVs
            sNaturalSkillDefense = naturalSkillDefense,//IVs
            sNaturalSkillSpAttack = naturalSkillSpAttack, //IVs
            sNaturalSkillSpDefense = naturalSkillSpDefense, //IVs
            sNaturalSkillSpeed = naturalSkillSpeed, //IVs
            sDevelopedSkillMaxHP = developedSkillMaxHP, //EVs
            sDevelopedSkillAttack = developedSkillAttack, //EVs
            sDevelopedSkillDefense = developedSkillDefense, //EVs
            sDevelopedSkillSpAttack = developedSkillSpAttack, //EVs
            sDevelopedSkillSpDefense = developedSkillSpDefense,//EVs
            sDevelopedSkillSpeed = developedSkillSpeed //EVs

        };

        return saveData;

    }



    float PersonalityValue(Stat stat)
    {   
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
        int OldMaxHP = MaxHP; //to keep track of max HP changes for things like evolution

        //HP has unique formula and is not effected by personality
        MaxHP = Mathf.FloorToInt
            (
                ( ( (2*Base.MaxHP + naturalSkillMaxHP + developedSkillMaxHP/4)* Level / 100  ) + Level + 10 ) 
            ); 

            if(OldMaxHP != 0)
            {
                HP += MaxHP - OldMaxHP;
            }

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
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0}
        };
    }

    public void ResetMoveLocks() //Reneables all moves
    {
        foreach(Move move in Moves)
        {
            if(move.IsDisabled)
            {
                move.DisableMove(false);
            }
        }
    }

    int GetStat(Stat stat)
    {
       int statValue = Stats[stat];

        
       int statStage = StatStages[stat];
       var stageModifiers = new float[] {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f}; //note that decreases are divided and boost are multiplies)
       var itemModifier = HeldItemStatModifier(stat); //returns modifiers based on held item
       
       //Stat Stage Changes
       if(statStage >= 0)
       {
            statValue = Mathf.FloorToInt(statValue * stageModifiers[statStage]);    
       }
       else if(statStage < 0 )
       {
           statValue = Mathf.FloorToInt(statValue / stageModifiers[-statStage]);//stat stage is negative so we negate in order to have valid index
       }

        //Held Item modifiers
        statValue = Mathf.FloorToInt(statValue * itemModifier);
        
        //Debug.Log($"{this.Base.MonsterName} {stat} was boosted by {this.HeldItem} by a value of {itemModifier}");
       
       return statValue; 
    }

    float HeldItemStatModifier(Stat stat)
    {
        
        
        if(HeldItem == null || HeldItem.IsEffectiveWhenHeld == false)
        {
            return 1;
        }
        else if(HeldItem is StatEnhancingItem)
        {
            StatEnhancingItem item = (StatEnhancingItem)HeldItem;

            if(item.PreventsStatusMoves) //items like assualt vest
            {
                foreach(Move move in Moves)
                {
                    if(move.Base.Category == MoveCategory.Status)
                    {
                        move.DisableMove(true);
                    }
                    else continue;
                }
            }
            return item.GetStatBoostModifier(stat);
        }

        return 1;
        
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
        if(Status == null)
        {
            Status = ConditionsDB.Conditions[conditionID];
            Status?.OnStart?.Invoke(this); //if the status has an on start method we will call it i.e. sleep
            StatusChangeMessages.Enqueue($"{Base.MonsterName} {Status.StartMessage}");
            OnStatusChanged?.Invoke();
        }
        else
        {
            StatusChangeMessages.Enqueue($"But it failed.");
            return;
        }
        


    }

    public void SetHeldItem(ItemBase item)
    {
        HeldItem = item;
        OnHeldItemChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();

    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if(VolatileStatus == null)
        {
            VolatileStatus = ConditionsDB.Conditions[conditionID];
            VolatileStatus?.OnStart?.Invoke(this); //if the status has an on start method we will call it i.e. sleep
            StatusChangeMessages.Enqueue($"{Base.MonsterName} {VolatileStatus.StartMessage}");

        }
        else
        {
            StatusChangeMessages.Enqueue($"But it failed.");
            return;
        }
        
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public void Protect(ConditionID conditionID)
    {
        
        if(ProtectedStatus == null)
        {   
            ProtectedStatus = ConditionsDB.Conditions[conditionID];

            if(ProtectedStatus.OnProtect(this)) //tries to protect target
            {
                IsProtected = true;
                StatusChangeMessages.Enqueue($"{Base.MonsterName} {ProtectedStatus.EffectMessage}");
            }
            else
            {
                StatusChangeMessages.Enqueue($"But it failed.");
                return;
            }
        }

        else // monster is already under protected status
        {
            StatusChangeMessages.Enqueue($"But it failed.");
            return;
        }
    }
    public void ResetProtect() //should reset protect values after each turn
    {
        if(ProtectedStatus != null && IsProtected) //monster protected this turn and did so sucesfully. We increment divide sucess chance by 3 each time.
        {
            ProtectSucessChance = ProtectSucessChance/3;
            IsProtected = false;
            ProtectedStatus = null;
        }
        else if(ProtectedStatus != null && !IsProtected)
        {
            ProtectSucessChance = 100f;
            ProtectedStatus = null;
        }
        else // monster did not have any protected status
        {
            return;
        }
    }

    public bool OnBeforeMove()
    {
        bool canMove = true;

       if(Status?.OnBeforeMove != null)//check for status
        {
            if(!Status.OnBeforeMove(this)) //if status exist roll for movement
            {
                canMove = false;
            }
        }
        if(VolatileStatus?.OnBeforeMove != null)//Check for volatile status
        {
            if(!VolatileStatus.OnBeforeMove(this)) //if status exist roll for movement
            {
                canMove = false;
            }
        }

        return canMove; //returning true when monster doesn't have a status that effects before move to allow the monster to move uneffected
    }
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this); //addomg a question mark after action will make sure that on after turn is not null
        VolatileStatus?.OnAfterTurn?.Invoke(this);
        ProtectedStatus?.OnAfterTurn?.Invoke(this);
        UseHeldItemsAfterTurn(); //Recovery Items like Black Sludge and LeftOvers
        
    }

    private void UseHeldItemsAfterTurn()
    {
        if (HeldItem == null || !HeldItem.IsEffectiveWhenHeld) return;
        else if(HeldItem is OnAfterTurnItem)
        {
            OnAfterTurnItem item = (OnAfterTurnItem)HeldItem;
            item.OnAfterTurn(this);
        }


    }



    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            CalculateStats();
            OnLevelChanged?.Invoke();
            return true;
        }
        
        return false;

    }

    public Evolution CheckForEvolution()
    {
        return Base.Evolutions.FirstOrDefault(e => e.EvolutionLevel <= level); //returns the form according to its level
        
    }

    public void Evolve(Evolution evolution)
    {
        
        _base = evolution.MonsterEvolution;
        CalculateStats();
        
    }
    public LearnableMove GetLearnableMoveAtCurrLevel()
    {
        return Base.LearnableMoves.Where(x => x.MoveLevel == level).FirstOrDefault();
    }

    public void LearnMove(MoveBase newMove)
    {
        if(Moves.Count > MonsterBase.MaxNumberOfMoves)
        {
            return;
        }

        Moves.Add(new Move(newMove));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0; //returns true if monster already has move
    }

    public void Heal() //fully heal monster
    {
        HP = MaxHP;
        OnHPChanged?.Invoke();

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
    
    public DamageDetails TakeDamage(Move attackerMove, Monster attackingMonster, Condition weather)
    {
        //critical hits
        float critical = 1f;
        if (Random.value * 100f <= 6.25f) critical = 2f;

        //type effectiveness
        float typeEffectiveness = TypeChart.GetEffectiveness(attackerMove.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(attackerMove.Base.Type, this.Base.Type2);

        //STAB bonus
        float stab = 1f;
        if (this.Base.Type1 == attackerMove.Base.Type || this.Base.Type2 == attackerMove.Base.Type) stab = 1.5f;

        //Weather modifier
        float weatherModifier = weather?.OnDamageModify?.Invoke(this, attackingMonster, attackerMove) ?? 1f; //note that if weather is null we return null

        //Item modifier
        float attackerItemModifier = attackingMonster.HeldItemDamageBoost(attackerMove, attackingMonster);

        var damageDetails = new DamageDetails()
        {
            CriticalDamageBonus = critical,
            TypeEffectiveness = typeEffectiveness
        };

        float attack = (attackerMove.Base.Category == MoveCategory.Special) ? attackingMonster.SpAttack : attackingMonster.Attack; //checks to see if move is special to determine attack type. This is a shorthand if statement
        float defense = (attackerMove.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        float modifiers = Random.Range(0.85f, 1f) * typeEffectiveness * critical * stab * weatherModifier * attackerItemModifier;
        float a = (2 * attackingMonster.Level + 10) / 250f;
        float d = a * attackerMove.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);
        bool isKO = ((HP - damage) < 1);

        //Damage Details post calc
        damageDetails.DamageAmount = damage;
        damageDetails.KO = isKO;
        
        //Damage Triggered items
        if(HeldItem != null && HeldItem.IsEffectiveWhenHeld)
        {
          DamageTriggeredHeldItems(damageDetails, attackerMove);
        } 

        Debug.Log(damageDetails.DamageAmount);
        return damageDetails;


    }

    

    public float HeldItemDamageBoost(Move attackerMove, Monster attackingMonster)
    {
        
        if(attackingMonster.HeldItem == null || attackingMonster.HeldItem.IsEffectiveWhenHeld == false)
        {
            return 1;
        }
        else if(attackingMonster.HeldItem is TypeEnhancingItem)
        {
            TypeEnhancingItem item = (TypeEnhancingItem)attackingMonster.HeldItem;
            return item.GetTypeBoostModifier(attackerMove);
        }
        else if(attackingMonster.HeldItem is StatEnhancingItem)
        {
            StatEnhancingItem item = (StatEnhancingItem)attackingMonster.HeldItem;

            if(item.LocksMoves)
            {
                foreach(Move move in attackingMonster.Moves)
                {
                    if(move == attackerMove)
                    {
                        continue;
                    }
                    else if(move.IsDisabled)
                    {
                        continue;
                    }
                    else //disables all other moves
                    {
                        move.DisableMove(true);
                    }
                }
            }
        }
        

        return 1;

    }
    void DamageTriggeredHeldItems(DamageDetails damageDetails, Move attackerMove)
    {
        if(HeldItem is Treat) //this monster has a treat
        {
            Treat treat = (Treat)HeldItem;
            treat.OnDamageTaken(damageDetails, attackerMove, this);
            
            
        }
        else if(HeldItem is OnDamageTriggerItem)
        {
            OnDamageTriggerItem item = (OnDamageTriggerItem)HeldItem;
            item.OnDamageTaken(damageDetails, attackerMove, this);
            
        }
        

    }

    public void RestoreHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHP);
        OnHPChanged?.Invoke();
    }

    public void RestoreHP(float percentage)
    {
        HP = Mathf.Clamp(Mathf.FloorToInt(HP+MaxHP*percentage), 0, MaxHP);
        OnHPChanged?.Invoke();
    }

    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        OnHPChanged?.Invoke();
    }

    public void OnBattleOver() //used to reset values after battle is over
    {
        VolatileStatus = null;
        InBattle = false;
        ResetStatStages();
        
    }
}

public class DamageDetails
{
    public bool KO{get; set;}
    public float CriticalDamageBonus{get; set;}
    public float TypeEffectiveness{get; set;}
    public int DamageAmount{get; set;}
}

[System.Serializable]
public class MonsterSaveData //only includes the savable data
{
    public string name; //the pokemon name will also be used to get all necessary base data
    public int sHp;
    public int sLevel;
    public int sExp;
    public string sHeldItemName; //We will use the name to restore the held item
    public ConditionID? sStatusId;
    public Personality sPersonality;
    public List<MoveSaveData> sMoves;
    public int sNaturalSkillMaxHP; //IVs
    public int sNaturalSkillAttack; //IVs
    public int sNaturalSkillDefense; //IVs
    public int sNaturalSkillSpAttack; //IVs
    public int sNaturalSkillSpDefense; //IVs
    public int sNaturalSkillSpeed; //IVs
    public int sDevelopedSkillMaxHP; //EVs
    public int sDevelopedSkillAttack; //EVs
    public int sDevelopedSkillDefense; //EVs
    public int sDevelopedSkillSpAttack; //EVs
    public int sDevelopedSkillSpDefense; //EVs
    public int sDevelopedSkillSpeed; //EVs


}