using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest{
    public QuestBase Base {get; private set;}
    public QuestStates Status {get;private set;}

    public Quest(QuestBase _base){
        Base = _base;
    }

    public IEnumerator StartQuest(){
        Status = QuestStates.Started;

        yield return DialogManager.Instance.ShowDialog(Base.StartDialog);

        var questList = QuestList.GetQuestList();   
        questList.AddQuest(this); 
    }

    public IEnumerator CompleteQuest(Transform player){
        Status = QuestStates.Completed;

        yield return DialogManager.Instance.ShowDialog(Base.CompletedDialog);

        var inventory = Inventory.GetInventory();
        if(Base.RequiredItem != null){
            inventory.RemoveItem(Base.RequiredItem);
        }

        if(Base.RewardItem != null){
            inventory.AddItem(Base.RewardItem);

            string playerName = player.GetComponent<PlayerController>().Name;
            yield return DialogManager.Instance.ShowDialogText($"{playerName} recieved {Base.RewardItem.Name}");
        }

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
    }

    public bool CanBeCompleted(){
        var inventory = Inventory.GetInventory();
        if(Base.RequiredItem != null){
            if(!inventory.HasItem(Base.RequiredItem)){
                return false;
            }
        }
        return true;
    }

    public Quest(QuestSaveData saveData){
        Base = QuestDB.GetObjectByName(saveData.name);
        Status = saveData.status;
    }

    public QuestSaveData GetSaveData(){
        var savedata = new QuestSaveData(){
            name = Base.name,
            status = Status
        };
        return savedata;
    }
}

[System.Serializable]
public class QuestSaveData{
    public string name;
    public QuestStates status;
}

public enum QuestStates { None, Started, Completed}
