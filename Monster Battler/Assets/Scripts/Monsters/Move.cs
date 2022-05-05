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
        AP = mBase.AP;

    }

    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetMoveByName(saveData.sMoveName);
        AP = saveData.sAP;

    }
    public MoveSaveData GetMoveSaveData()
    {
        var saveData = new MoveSaveData()
        {
            sMoveName = Base.MoveName,
            sAP = AP
        };

        return saveData;

    }

    public void RestoreAP(int amount)
    {
        AP = Mathf.Clamp(AP + amount, 0, Base.AP);
    }
}

[System.Serializable]
public class MoveSaveData
{
    public string sMoveName;
    public int sAP;
}

