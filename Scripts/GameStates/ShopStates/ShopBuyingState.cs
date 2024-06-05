using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class ShopBuyingState : State<GameController>{
    GameController gameController;
    Inventory inventory;
    bool browseItems = false;

    public static ShopBuyingState i{get; private set;}

    //Inputs
    [SerializeField] Vector2 shopCameraOffSet;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CounterSelectorUI counterSelectorUI;
    [SerializeField] ShopUI shopUI;

    public List<ItemBase> AvailableItems {get; set;}

    private void Awake(){
        i = this;
    }

    public void Start(){
        inventory = Inventory.GetInventory();
    }

    public override void Enter(GameController owner){
        gameController = owner;

        browseItems = false;
        StartCoroutine(StartBuyingState());
    }

    IEnumerator StartBuyingState(){
        yield return GameController.Instance.MoveCamera(shopCameraOffSet);

        walletUI.Show();
        shopUI.Show(AvailableItems, (item) => StartCoroutine(BuyItem(item)), () => StartCoroutine(OnBackForBuying()));
        
        browseItems = true;
    }

    IEnumerator BuyItem(ItemBase item){
        browseItems = false;
        yield return DialogManager.Instance.ShowDialogText($"How many {item.Name}s would you like to buy?", waitForInput: false, autoClose: false);
        
        int countToBuy = 1;
        yield return counterSelectorUI.ShowSelector(100, item.Price, (selectedCount) => countToBuy = selectedCount);

        DialogManager.Instance.CloseDialog();

        var totalPrice = item.Price * countToBuy;
        if(Wallet.i.HasMoney(totalPrice)){
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Its will be cost you {totalPrice}$.",
                waitForInput: false, choices: new List<string>() {"Buy", "Leave"}, onChoiceSelected:choiceIndex => selectedChoice = choiceIndex);
            
            if(selectedChoice == 0){
                //Yes
                inventory.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogManager.Instance.ShowDialogText($"Thank you for shopping with us.");
            }
        } else {
            yield return DialogManager.Instance.ShowDialogText($"You don't have enough money for that!");
        }

        browseItems = true;
    }

    IEnumerator OnBackForBuying(){
        yield return GameController.Instance.MoveCamera(-shopCameraOffSet);
        shopUI.Close();
        walletUI.Close();

        gameController.StateMachine.Pop();
    }

    public override void Execute(){
        if (browseItems){
            shopUI.HandleUpdate();
        }
    }
}
