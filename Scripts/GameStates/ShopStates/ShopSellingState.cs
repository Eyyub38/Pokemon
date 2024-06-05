using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GDEUtills.StateMachine;

public class ShopSellingState : State<GameController>{
    GameController gameController;
    Inventory inventory;

    //Input
    public List<ItemBase> AvailableItems{get; set;}
    [SerializeField] WalletUI walletUI;
    [SerializeField] CounterSelectorUI counterSelectorUI;

    public static ShopSellingState i{get; private set;}

    public void Start(){
        inventory = Inventory.GetInventory();
    }

    private void Awake(){
        i = this;
    }

    public override void Enter(GameController owner){
        gameController = owner;

        StartCoroutine(StartSellingState());
    }

    IEnumerator StartSellingState(){
        yield return gameController.StateMachine.PushAndWait(InventoryState.i);

        var seletedItem = InventoryState.i.SelectedItem;

        if(seletedItem != null){
            yield return SellItem(seletedItem);
            StartCoroutine(StartSellingState());
        } else {
            gameController.StateMachine.Pop();
        }
    }

    IEnumerator SellItem(ItemBase item){
        if(!item.IsSellable){
            yield return DialogManager.Instance.ShowDialogText("You cannot sell this item!");
            yield break;
        }

        walletUI.Show();

        float sellingPrice = Mathf.Round(item.Price * 2 / 3);

        var itemCount = inventory.GetItemCount(item);

        int countToSell = 1;

        if(itemCount > 1){
            yield return DialogManager.Instance.ShowDialogText($"How many {item.Name}s would you like to sell?",
                waitForInput: false, autoClose: false);

            yield return counterSelectorUI.ShowSelector(itemCount, sellingPrice, (selectedCount) => countToSell = selectedCount);

            DialogManager.Instance.CloseDialog();
        }

        sellingPrice = sellingPrice * countToSell;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"I can give you {sellingPrice}$ for {item.Name}",
            waitForInput: false, choices: new List<string>() {"Sell", "Leave"}, onChoiceSelected:choiceIndex => selectedChoice = choiceIndex);

        if(selectedChoice == 0){
            inventory.RemoveItem(item, countToSell);
            Wallet.i.AddMoney(sellingPrice);
            yield return DialogManager.Instance.ShowDialogText($"Turned ove {item.Name} an recieved {sellingPrice}$");
        }

        walletUI.Close();
    }
}
