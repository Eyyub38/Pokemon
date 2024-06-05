using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GameMenuState : State<GameController>{
    [SerializeField] MenuController menuController;
    public static GameMenuState i{get; private set;}

    private void Awake(){
        i = this;
    }

    GameController gameController;
    public override void Enter(GameController owner){
        gameController = owner;
        menuController.gameObject.SetActive(true);

        menuController.OnSelected += OnMenuItemSelected;
        menuController.OnBack += OnBack;
    }

    void OnMenuItemSelected(int selection){
        if(selection == 0){//Pokemon
            gameController.StateMachine.Push(PartyState.i);
        } else if(selection == 1){//Bag
            gameController.StateMachine.Push(InventoryState.i);
        } else if(selection == 4){//Boxes
            gameController.StateMachine.Push(StorageState.i);
        }
    }

    void OnBack(){
        gameController.StateMachine.Pop();
    }

    public override void Execute(){
        menuController.HandleUpdate();

    }

    public override void Exit(){
        menuController.gameObject.SetActive(false);

        menuController.OnSelected -= OnMenuItemSelected;
        menuController.OnBack -= OnBack;
    }

}
