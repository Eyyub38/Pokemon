using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class ActionSelectionState : State<BattleSystem>{
    [SerializeField] ActionSelectionUI actionSelectionUI;

    BattleSystem battleSystem;

    public static ActionSelectionState i{get; private set;}

    private void Awake(){
        i = this;
    }
    public override void Enter(BattleSystem owner){
        battleSystem = owner;

        actionSelectionUI.gameObject.SetActive(true);
        actionSelectionUI.OnSelected += OnActionSelected;

        battleSystem.DialogBox.SetDialog("Choose an action.");
    }

    public override void Execute(){
        actionSelectionUI.HandleUpdate();
    }

    void OnActionSelected(int selection){
        if(selection == 0){
            //Fight
            battleSystem.SelectedAction = BattleAction.Move;
            MoveSelectionState.i.Moves = battleSystem.PlayerUnit.Pokemon.Moves;
            battleSystem.StateMachine.ChangeState(MoveSelectionState.i);
        } else if(selection == 1){
            //Bag
            StartCoroutine(GoToInventoryState());
        } else if(selection == 2){
            //Pokemon
            StartCoroutine(GoToPartyState());
        } else if(selection == 3){
            //Escape
            battleSystem.SelectedAction = BattleAction.Run;
            battleSystem.StateMachine.ChangeState(RunTurnState.i);
        }
    }

    public override void Exit(){
        actionSelectionUI.gameObject.SetActive(false);
        actionSelectionUI.OnSelected -= OnActionSelected;
    }

    IEnumerator GoToPartyState(){
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
        var selectedPokemon = PartyState.i.SelectedPokemon;

        if(selectedPokemon != null){
            battleSystem.SelectedAction = BattleAction.SwitchPokemon;
            battleSystem.SelectedPokemon = selectedPokemon;
            battleSystem.StateMachine.ChangeState(RunTurnState.i);
        }
    }

    IEnumerator GoToInventoryState(){
        yield return GameController.Instance.StateMachine.PushAndWait(InventoryState.i);
        var selectedItem = InventoryState.i.SelectedItem;

        if(selectedItem != null){
            battleSystem.SelectedAction = BattleAction.UseItem;
            battleSystem.SelectedItem = selectedItem;
            battleSystem.StateMachine.ChangeState(RunTurnState.i);
        }        
    }
}
