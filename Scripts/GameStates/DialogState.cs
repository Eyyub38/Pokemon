using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class DialogState : State<GameController>{
    public static DialogState i{get; private set;}

    GameController gameController;

    private void Awake(){
        i = this;
    }

    public override void Execute(){
        DialogManager.Instance.HandleUpdate();
    }
}
