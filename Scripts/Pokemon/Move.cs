using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class Move
{
    public MoveBase Base { get; set; }

    public int PP { get; set; }

    public Move(MoveBase pBase){
        Base = pBase;
        PP = pBase.PP;
    }

    public Move(MoveSaveData saveData){
        Base = MoveDB.GetObjectByName(saveData.name);
        PP = saveData.PP;
    }

    public MoveSaveData GetSaveData(){
        var saveData = new MoveSaveData{
            name = Base.name,
            PP = Base.PP
        };
        return saveData;
    }

    public void IncreasePP(int amount){
        PP = Mathf.Clamp(PP + amount, 0, Base.PP);
    }
}

[Serializable]
public class MoveSaveData{
    public string name;
    public int PP;
}
