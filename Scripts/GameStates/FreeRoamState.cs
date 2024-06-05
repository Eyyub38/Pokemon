using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class FreeRoamState : State<GameController>{

    public static FreeRoamState i{get; private set;}

    private void Awake(){
        i = this;
    }

    GameController gameController;
    public override void Enter(GameController owner){
        gameController = owner;
    }

    public override void Execute(){
        PlayerController.i.HandleUpdate();

        if(Input.GetKeyDown(KeyCode.Tab)){
            gameController.StateMachine.Push(GameMenuState.i);
        }
    }
}
