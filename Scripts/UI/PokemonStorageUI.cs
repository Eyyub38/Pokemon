using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDE.GenericSelectionUI;
using UnityEngine;
using UnityEngine.UI;

public class PokemonStorageUI : SelectionUI<ImageSlot>{
    [SerializeField] List<ImageSlot> boxSlots;
    [SerializeField] Image movingPokemon;

    PokemonParty party;
    PokemonStorageBoxes storageBoxes;
    int totalColumns = 7;

    public int SelectedBox {get; private set;} = 0;

    List<BoxPartySlotUI> partySlots = new List<BoxPartySlotUI>();
    List<BoxStorageUI> storageSlots = new List<BoxStorageUI>();

    List<Image> boxSlotimages = new List<Image>();

    private void Awake(){
        foreach(var boxSlot in boxSlots){
            var storageSlot = boxSlot.GetComponent<BoxStorageUI>();

            if(storageSlot != null){
                storageSlots.Add(storageSlot);
            } else {
                partySlots.Add(boxSlot.GetComponent<BoxPartySlotUI>());
            }
        }

        party = PokemonParty.GetPlayerParty();
        storageBoxes = PokemonStorageBoxes.GetPlayerStorageBoxes();

        boxSlotimages = boxSlots.Select( b => b.transform.GetChild(0).GetComponent<Image>()).ToList();
        movingPokemon.gameObject.SetActive(false);
    }

    void Start(){
        SetItems(boxSlots);
        SetSelectionSettings(SelectionType.Grid, 7);
    }

    public void SetDataInPartySlots(){
        for(int i = 0; i < partySlots.Count; i++){
            if(i < party.Pokemons.Count){
                partySlots[i].SetData(party.Pokemons[i]);
            } else {
                partySlots[i].ClearData();
            }
        }
    }
    public void SetDataInStorageSlots(){
        for(int i = 0; i < storageSlots.Count; i++){
            var pokemon = storageBoxes.GetPokemon(SelectedBox, i);
            if(pokemon != null){
                storageSlots[i].SetData(pokemon);
            } else {
                storageSlots[i].ClearData();
            }
        }
    }

    public bool IsPartySlot(int slotIndex){
        return slotIndex % totalColumns == 0;
    }

    public Pokemon TakePokemonFromSlot(int slotIndex){
        Pokemon pokemon;
        if(IsPartySlot(slotIndex)){
            int partyIndex = slotIndex / totalColumns;

            if(partyIndex >= party.Pokemons.Count){
                return null;
            }

            pokemon = party.Pokemons[partyIndex];
            party.Pokemons[partyIndex] = null;
        } else {
            int boxSlotIndex = slotIndex - (slotIndex / totalColumns + 1);
            pokemon = storageBoxes.GetPokemon(SelectedBox, boxSlotIndex);

            storageBoxes.RemovePokemon(SelectedBox, boxSlotIndex);
        }
    
        movingPokemon.sprite = boxSlotimages[slotIndex].sprite;
        movingPokemon.transform.position = boxSlotimages[slotIndex].transform.position + Vector3.up * 50f;
        boxSlotimages[slotIndex].color = new Color( 1, 1, 1, 0);
        movingPokemon.gameObject.SetActive(true);

        return pokemon;
    }

    public void PutPokemonIntoSlot(Pokemon pokemon, int slotIndex){
        if(IsPartySlot(slotIndex)){
            int partyIndex = slotIndex / totalColumns;

            if(partyIndex >= party.Pokemons.Count){
                party.Pokemons.Add(pokemon);
            } else {
                party.Pokemons[partyIndex] = pokemon;
            }
        } else {
            int boxSlotIndex = slotIndex - (slotIndex / totalColumns + 1);
            storageBoxes.AddPokemon(pokemon, SelectedBox, boxSlotIndex);
        }

        movingPokemon.gameObject.SetActive(false);
    }

    public override void UpdateSelectionInUI(){
        base.UpdateSelectionInUI();

        if(movingPokemon.gameObject.activeSelf){
            movingPokemon.transform.position = boxSlotimages[selectedItem].transform.position + Vector3.up * 50f;
        }
    }
}
