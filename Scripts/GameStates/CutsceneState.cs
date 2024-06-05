using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class CutsceneState : State<GameController>{
    public static CutsceneState i{get; private set;}

    private void Awake(){
        i = this;
    }

    public override void Execute(){
        PlayerController.i.Character.HandleUpdate();
    }
}
