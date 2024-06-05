using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestList : MonoBehaviour, ISavable{
    List<Quest> quests = new List<Quest>();

    public event Action OnUpdated;

    public void AddQuest(Quest quest){
        if(!quests.Contains(quest)){
            quests.Add(quest);
        }

        OnUpdated?.Invoke();
    }

    
    public bool IsStarted(string questName){
        var questStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return questStatus == QuestStates.Started || questStatus == QuestStates.Completed;
    }

    public bool IsCompleted(string questName){
        var questStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return questStatus == QuestStates.Completed;
    }

    public static QuestList GetQuestList(){
        return FindObjectOfType<PlayerController>().GetComponent<QuestList>();
    }

    public object CaptureState()
    {
        return quests.Select(q => q.GetSaveData()).ToList();
    }

    public void RestoreState(object state)
    {
        var saveData = state as List<QuestSaveData>;
        if(saveData != null){
            quests = saveData.Select(s => new Quest(s)).ToList();
            OnUpdated?.Invoke();
        }
    }
}