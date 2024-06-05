using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class DynamicMenuState : State<GameController>{  
    [SerializeField] DynamicMenuUI dynamicMenuUI;
    [SerializeField] TextSlot itemTextPrefab;
    //Input
    public List<string> MenuItems {get; set;}
    //Output
    public int? SelectedItem {get; private set;}
    
    public static DynamicMenuState i {get; private set;}
    GameController gameController;

    private void Awake(){
        i = this;
    }

    public override void Enter(GameController owner){
        gameController = owner;

        foreach (Transform child in dynamicMenuUI.transform){
            Destroy(child.gameObject);
        }

        var itemTextSlots = new List<TextSlot>();
        foreach (var menuItems in MenuItems){
            var itemTextSlot = Instantiate(itemTextPrefab, dynamicMenuUI.transform);
            itemTextSlot.SetText(menuItems);
            itemTextSlots.Add(itemTextSlot);
        }

        dynamicMenuUI.SetItems(itemTextSlots);

        dynamicMenuUI.gameObject.SetActive(true);
        dynamicMenuUI.OnSelected += OnItemSelect;
        dynamicMenuUI.OnBack += OnBack;
    }

    void OnItemSelect(int selection){
        SelectedItem = selection;
        gameController.StateMachine.Pop();
    }

    void OnBack(){
        SelectedItem = null;
        gameController.StateMachine.Pop();
    }

    public override void Execute(){
        dynamicMenuUI.HandleUpdate();
    }

    public override void Exit(){
        dynamicMenuUI.ClaerItems();
        
        dynamicMenuUI.gameObject.SetActive(false);
        dynamicMenuUI.OnSelected -= OnItemSelect;
        dynamicMenuUI.OnBack -= OnBack;
    }
}
