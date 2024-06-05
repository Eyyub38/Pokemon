using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GDEUtills.StateMachine;

public class MoveToForgetState : State<GameController>{
    [SerializeField] MoveToForgetSelectionUI moveSelectionUI;

    public static MoveToForgetState i{get; private set;}

    GameController gameController;

    //Inputs
    public List<MoveBase> CurrentMoves {get; set;}
    public MoveBase NewMove{get; set;}
    //Output
    public int Selection {get;set;}

    
    private void Awake(){
        i = this;
    }

    public override void Enter(GameController owner){
        gameController = owner;

        Selection = 0;

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(CurrentMoves, NewMove);

        moveSelectionUI.OnSelected += OnMoveSelected;
        moveSelectionUI.OnBack += OnBack;
    }

    void OnMoveSelected(int selection){
        Selection = selection;
        gameController.StateMachine.Pop();
    }

    void OnBack(){
        Selection = -1;
        gameController.StateMachine.Pop();
    }

    public override void Execute(){
        moveSelectionUI.HandleUpdate();
    }

    public override void Exit(){
        moveSelectionUI.gameObject.SetActive(false);
        moveSelectionUI.OnSelected -= OnMoveSelected;
        moveSelectionUI.OnBack -= OnBack;
    }
}
