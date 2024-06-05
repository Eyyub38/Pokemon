using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class InventoryState : State<GameController>{
    [SerializeField] InventoryUI inventoryUI;
    
    GameController gameController;
    Inventory inventory;

    public static InventoryState i{ get; private set;}
    
    //Output
    public ItemBase SelectedItem{ get; private set;}

    private void Start(){
        inventory = Inventory.GetInventory();
    }

    private void Awake(){
        i = this;
    }

    public override void Enter(GameController owner){
        gameController = owner;

        SelectedItem = null;
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OnSelected += OnItemSelected;
        inventoryUI.OnBack += OnBack;    
    }

    void OnItemSelected(int selection){
        SelectedItem = inventoryUI.SelectedItem;
        
        if(gameController.StateMachine.GetPreviousState() != ShopSellingState.i){
            StartCoroutine(SelectPokemonAndUseItem());
        } else {
            gameController.StateMachine.Pop();
        }     
    }

    void OnBack(){
        SelectedItem = null;
        gameController.StateMachine.Pop();
    }

    public override void Execute(){
        inventoryUI.HandleUpdate();
    }

    public override void Exit(){
        inventoryUI.gameObject.SetActive(false);
        inventoryUI.OnSelected -= OnItemSelected;
        inventoryUI.OnBack -= OnBack;
    }

    IEnumerator SelectPokemonAndUseItem(){
        var prevState = gameController.StateMachine.GetPreviousState();

        if(prevState == BattleState.i){
            //In Battle
            if(!SelectedItem.canUsedInBattle){
                yield return DialogManager.Instance.ShowDialogText($"{SelectedItem.Name} cannot bu used in battle!");
                yield break;
            }
        } else {
            //Out Side Battle
            if(!SelectedItem.canUsedOutsideBattle){
                yield return DialogManager.Instance.ShowDialogText($"{SelectedItem.Name} can be used only in battle!");
                yield break;
            }
        }

        if(SelectedItem is PokeballItem){
            inventory.UseItem(SelectedItem, null);
            gameController.StateMachine.Pop();
            yield break;
        }

        yield return gameController.StateMachine.PushAndWait(PartyState.i);

        if(prevState == BattleState.i){
            if(UseItemState.i.ItemUsed){
                gameController.StateMachine.Pop();
            }
        }
    }
}
