using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class PauseState : State<GameController>{
    public static PauseState i{get; private set;}
    private void Awake(){
        i = this;
    }


}
