using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    [SerializeField] MonsterBase _base;
    [SerializeField] int level;
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

    public void Init() //this method creates our pokemon
    {
        HP = MaxHP;

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
    }

    

    //Stat Properties
    
    public int MaxHP
    {
        get { return Mathf.FloorToInt( (Base.MaxHP * Level) / 100f ) + 10; }
    }
    public int Attack
    {
        get { return Mathf.FloorToInt( (Base.Attack * Level) / 100f ) + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt( (Base.Defense * Level) / 100f ) + 5; }
    }
    public int SpAttack
    {
        get { return Mathf.FloorToInt( (Base.SpAttack * Level) / 100f ) + 5; }
    } 
    public int SpDefense
    {
        get { return Mathf.FloorToInt( (Base.SpDefense * Level) / 100f ) + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt( (Base.Speed * Level) / 100f ) + 5; }
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

        float attack = (move.Base.IsSpecial) ? attacker.SpAttack : attacker.Attack; //checks to see if move is special to determine attack type. This is a shorthand if statement
        float defense = (move.Base.IsSpecial)? SpDefense : Defense;

        float modifiers = Random.Range(0.85f, 1f) * typeEffectiveness * critical * stab;
        float a = (2*attacker.Level + 10)/ 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d*modifiers);

        

        HP -= damage;
        if(HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
            
        }
        
        return damageDetails;

        
    }
}

public class DamageDetails
{
    public bool Fainted{get; set;}
    public float Critical{get; set;}
    public float TypeEffectiveness{get; set;}
}
