using System.Linq;
using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class UseItemState : State<GameController>{
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    public static UseItemState i{get; private set;}
    
    //Output
    public bool ItemUsed {get; private set;}

    GameController gameController;
    Inventory inventory;

    private void Awake(){
        i = this;
        ItemUsed = false;
        inventory = Inventory.GetInventory();
    }

    public override void Enter(GameController owner){
        gameController = owner;

        StartCoroutine(UseItem());
    }

    IEnumerator UseItem(){
        var item = inventoryUI.SelectedItem;
        var pokemon = partyScreen.SelectedMember;

        if(item is TmItem){
            yield return HandleTmItems();
        } else {
            //Handle Evolution Item
            if(item is EvolutionItem){
                var evolution = pokemon.CheckForEvolution(item);
                if(evolution != null){
                    yield return EvolutionState.i.Evolve(pokemon, evolution);
                } else {
                    yield return DialogManager.Instance.ShowDialogText($"It won't have any affect!");
                    gameController.StateMachine.Pop();
                    yield break;
                }
            }

            var usedItem = inventory.UseItem(item, partyScreen.SelectedMember);
            if(usedItem != null){
                ItemUsed = true;
                if(usedItem is RecoveryItems){
                    yield return DialogManager.Instance.ShowDialogText($"Player used {usedItem.Name}.");
                }
            } else {
                if(inventoryUI.SelectedCategory == (int)ItemCategory.Items){
                    yield return DialogManager.Instance.ShowDialogText($"It won't have any affect!");
                }
            }
        }
        
        gameController.StateMachine.Pop();
    }

    IEnumerator HandleTmItems(){
        var tmItem= inventoryUI.SelectedItem as TmItem;
        if(tmItem == null){
            yield break;
        }

        var pokemon = partyScreen.SelectedMember;

        if(pokemon.HasMove(tmItem.Move)){
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} already knows {tmItem.Move.Name}");
            yield break;
        }

        if(!tmItem.CanBeTaught(pokemon)){
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} can't' learn {tmItem.Move.Name}");
            yield break;
        }

        if(pokemon.Moves.Count < PokemonBase.MaxNumberOfMoves){
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} learned {tmItem.Move.Name}");
        } else {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is trying learn {tmItem.Move.Name}");
            yield return DialogManager.Instance.ShowDialogText($"But it can't learns more than {PokemonBase.MaxNumberOfMoves} moves");

            yield return DialogManager.Instance.ShowDialogText($"Choose a move you want to forget", true, false);

            MoveToForgetState.i.NewMove = tmItem.Move;
            MoveToForgetState.i.CurrentMoves = pokemon.Moves.Select(m => m.Base).ToList();
            yield return gameController.StateMachine.PushAndWait(MoveToForgetState.i);
            
            int moveIndex = MoveToForgetState.i.Selection;

            if(moveIndex == PokemonBase.MaxNumberOfMoves || moveIndex == -1){
                //Don't learn any move
                yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {tmItem.Move.Name}.");
            } else {
                //Forget the selected move and learn the new move
                var selectedMove = pokemon.Moves[moveIndex].Base;
                yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot {selectedMove.Name} and learn {tmItem.Move.Name}");
                pokemon.Moves[moveIndex] = new Move(tmItem.Move);
            }
        }
    }
}
