using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase Base { get; set;}
    public int AP {get; set;}

    public Move(MoveBase mBase)
    {
        Base = mBase;
        AP = mBase.Ap;

    }
}

