using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class MoveSelectionState : State<BattleSystem>{
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] GameObject moveDetailsUI;

    //Input
    public List<Move> Moves{get; set;}

    BattleSystem battleSystem;

    public static MoveSelectionState i{get; private set;}

    private void Awake(){
        i = this;
    }
    

    public override void Enter(BattleSystem owner){
        battleSystem = owner;

        moveSelectionUI.SetMoves(Moves);
        
        moveSelectionUI.gameObject.SetActive(true);

        moveSelectionUI.OnSelected += OnMoveSelected;
        moveSelectionUI.OnBack += OnBack;

        moveDetailsUI.gameObject.SetActive(true);
        battleSystem.DialogBox.EnableDialogText(false);
    }

    public override void Execute(){
        moveSelectionUI.HandleUpdate();
    }

    void OnMoveSelected(int selection){
        battleSystem.SelectedMove = selection;
        battleSystem.StateMachine.ChangeState(RunTurnState.i);
    }

    void OnBack(){
        battleSystem.StateMachine.ChangeState(ActionSelectionState.i);
    }

    public override void Exit(){
        moveSelectionUI.gameObject.SetActive(false);

        moveSelectionUI.OnSelected -= OnMoveSelected;
        moveSelectionUI.OnBack -= OnBack;

        moveSelectionUI.ClaerItems();

        moveDetailsUI.gameObject.SetActive(false);
        battleSystem.DialogBox.EnableDialogText(true);
    }
}
