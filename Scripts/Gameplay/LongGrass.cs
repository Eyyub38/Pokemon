using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerRepeatdly => true;

    public void OnPlayerTriggered(PlayerController player){
        if(Random.Range(1,101)<=10){
                player.Character.Animator.IsMoving = false;
                GameController.Instance.StartBattle(BattleTrigger.LongGrass);
        }
    }
}
