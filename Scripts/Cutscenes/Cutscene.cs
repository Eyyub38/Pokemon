using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Cutscene : MonoBehaviour, IPlayerTriggerable{
    [SerializeReference]
    [SerializeField] List<CutsceneAction> actions;

    public bool TriggerRepeatdly => false;
    
    public void OnPlayerTriggered(PlayerController player){
        player.Character.Animator.IsMoving = false;

        StartCoroutine(Play());
    }

    public void AddAction(CutsceneAction action){
        #if UNITY_EDITOR//If in compile unity run this code or in build pass it.
            Undo.RegisterCompleteObjectUndo(this, "Add action to cutscene");
        #endif

        action.Name = action.GetType().ToString();
        actions.Add(action);
    }


    public IEnumerator Play(){
        GameController.Instance.StateMachine.Push(CutsceneState.i);
        foreach (var action in actions){
            if(action.WaitForCompletion){
                yield return action.Play();
            } else {
                StartCoroutine(action.Play());
            }
        }
        
        GameController.Instance.StateMachine.Pop();
    }
}
