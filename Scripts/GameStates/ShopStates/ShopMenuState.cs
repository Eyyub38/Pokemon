using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class ShopMenuState : State<GameController>{

    GameController gameController;
    //Input
    public List<ItemBase> AvailableItems{get; set;}

    public static ShopMenuState i{get; private set;}

    private void Awake(){
        i = this;
    }

    public override void Enter(GameController owner){
        gameController = owner;

        StartCoroutine(StartMenuState());

    }

    IEnumerator StartMenuState(){

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("Welcome to the PokeMart. How can I help you?", waitForInput: false,
            choices: new List<string>() {"Buy","Sell","Leave"},
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
        if(selectedChoice == 0){
            //Buy
            ShopBuyingState.i.AvailableItems = AvailableItems;
            yield return gameController.StateMachine.PushAndWait(ShopBuyingState.i);

        } else if(selectedChoice == 1){
            //Sell
            yield return gameController.StateMachine.PushAndWait(ShopSellingState.i);
        } else if(selectedChoice == 2){
            //Leave
        }

        gameController.StateMachine.Pop();
    }
}
