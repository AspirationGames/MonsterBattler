using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase Base { get; set;}
    public int AP {get; set;}
    public bool IsDisabled {get; private set;}

    public Move(MoveBase mBase)
    {
        Base = mBase;
        AP = mBase.AP;

    }

    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetObjectByName(saveData.name);
        AP = saveData.sAP;

    }
    public MoveSaveData GetMoveSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = Base.name,
            sAP = AP
        };

        return saveData;

    }

    public void DisableMove(bool disable)
    {
        IsDisabled = disable;
    }

    public void RestoreAP(int amount)
    {
        AP = Mathf.Clamp(AP + amount, 0, Base.AP);
    }
}

[System.Serializable]
public class MoveSaveData
{
    public string name;
    public int sAP;
}

