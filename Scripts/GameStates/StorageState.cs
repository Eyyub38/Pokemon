using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class StorageState : State<GameController>{
    [SerializeField] PokemonStorageUI storageUI;

    GameController gameController;
    PokemonParty party;
    bool isMovingPokemon = false;
    Pokemon selectedPokemonToMove = null;
    int selectedSlotToMove = 0;

    public static StorageState i { get; private set;}

    private void Awake(){
        i = this;
        party = PokemonParty.GetPlayerParty();
    }

    public override void Enter(GameController owner){
        gameController = owner;

        storageUI.gameObject.SetActive(true);
        storageUI.SetDataInPartySlots();
        storageUI.SetDataInStorageSlots();

        storageUI.OnSelected += OnSlotSelected;
        storageUI.OnBack += OnBack;
    }

    void OnSlotSelected(int slotIndex){
        if(!isMovingPokemon){
            var pokemon = storageUI.TakePokemonFromSlot(slotIndex);

            if(pokemon != null){
                isMovingPokemon = true;

                selectedSlotToMove = slotIndex;
                selectedPokemonToMove = pokemon;
            }
        } else {
            isMovingPokemon = false;

            int firstSlotIndex = selectedSlotToMove;
            int secondSlotIndex = slotIndex;

            var secondPokemon = storageUI.TakePokemonFromSlot(slotIndex);

            if(secondPokemon == null && storageUI.IsPartySlot(firstSlotIndex) && storageUI.IsPartySlot(secondSlotIndex)){
                storageUI.PutPokemonIntoSlot(selectedPokemonToMove, selectedSlotToMove);

                storageUI.SetDataInPartySlots();
                storageUI.SetDataInStorageSlots();
                return;
            }
            storageUI.PutPokemonIntoSlot(selectedPokemonToMove, secondSlotIndex);

            if(secondPokemon != null){
                storageUI.PutPokemonIntoSlot(secondPokemon, firstSlotIndex);
            }

            party.Pokemons.RemoveAll(p => p == null);
            party.PartyUpdated();

            storageUI.SetDataInPartySlots();
            storageUI.SetDataInStorageSlots();
        }
    }

    void OnBack(){
        if(isMovingPokemon){
            isMovingPokemon = false;
            storageUI.PutPokemonIntoSlot(selectedPokemonToMove, selectedSlotToMove);
        } else {
            gameController.StateMachine.Pop();
        }
    }

    public override void Execute(){
        storageUI.HandleUpdate();
    }

    public override void Exit(){
        storageUI.gameObject.SetActive(false);
        
        storageUI.OnSelected -= OnSlotSelected;
        storageUI.OnBack -= OnBack;
    }
}
