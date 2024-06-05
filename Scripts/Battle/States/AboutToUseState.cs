using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class AboutToUseState : State<BattleSystem>{
    BattleSystem battleSystem;
    //Input
    public Pokemon NewPokemon {get; set;}
    bool aboutToUseChoice;

    public static AboutToUseState i{get; private set;}

    private void Awake(){
        i = this;
    }

    public override void Enter(BattleSystem owner){
        battleSystem = owner;
        StartCoroutine(StartState());
    }

    IEnumerator StartState(){
        yield return battleSystem.DialogBox.TypeDialog($"{battleSystem.Trainer.Name} is about to use {NewPokemon.Base.Name}. Do you wanna change your pokemon?");
        battleSystem.DialogBox.EnableChoiceBox(true);
    }

    public override void Execute(){
        if(!battleSystem.DialogBox.IsChoiceBoxEnabled){
            return;
        }
        
        if(Input.GetKeyDown( KeyCode.UpArrow ) || Input.GetKeyDown( KeyCode.DownArrow )){
            aboutToUseChoice =! aboutToUseChoice;
        }
        battleSystem.DialogBox.UpdateChoiceBox( aboutToUseChoice );

        if(Input.GetKeyDown( KeyCode.Return )){
            battleSystem.DialogBox.EnableChoiceBox( false );

            if(aboutToUseChoice == true){;
                //Yes Option
                StartCoroutine(SwitchAndCountinueBattle());
            } else {
                //No Option
                StartCoroutine(CountinueBattle());
            }

        } else if(Input.GetKeyDown( KeyCode.Backspace )){
            battleSystem.DialogBox.EnableChoiceBox( false );
            StartCoroutine(CountinueBattle());
        }
    }

    IEnumerator CountinueBattle(){
        yield return battleSystem.SendNextTrainerPokemon();
        battleSystem.StateMachine.Pop();
    }

    IEnumerator SwitchAndCountinueBattle(){
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
        var selectedPokemon = PartyState.i.SelectedPokemon;
        if(selectedPokemon != null){
            yield return battleSystem.SwitchPokemon(selectedPokemon);
        }

        yield return CountinueBattle();
    }
}
