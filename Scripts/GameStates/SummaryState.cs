using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;
using System;

public class SummaryState : State<GameController>{
    [SerializeField] SummaryScreenUI summaryScreen;
    
    //Input
    public int SelectedPokemonIndex {get; set;}
    List<Pokemon> playerParty;

    GameController gameController;
    int selectedPage = 0;

    public static SummaryState i {get; private set;}

    private void Awake(){
        i = this;
    }

    private void Start(){
        playerParty = PlayerController.i.GetComponent<PokemonParty>().Pokemons;
    }

    public override void Enter(GameController owner){
        gameController = owner;

        summaryScreen.gameObject.SetActive(true);
        summaryScreen.SetBasicDetails(playerParty[SelectedPokemonIndex]);
        summaryScreen.ShowPage(selectedPage);
    }

    public override void Execute(){
        int prevSelection = SelectedPokemonIndex;
        int prevPage = selectedPage;

        if(!summaryScreen.InMoveSelection){
            //Page Selection
            if(Input.GetButtonDown("Detail")){
                selectedPage = Mathf.Abs(selectedPage + 1) % 2;
            } else if(Input.GetButtonDown("Main")){
                selectedPage = Mathf.Abs(selectedPage - 1) % 2;
            }

            if(selectedPage != prevPage){
                summaryScreen.ShowPage(selectedPage);
            }
            
            //Pokemon Selection
            if(Input.GetButtonDown("Next")){
                SelectedPokemonIndex += 1;

                if(SelectedPokemonIndex >= playerParty.Count){
                    SelectedPokemonIndex = 0;
                }
                
            } else if(Input.GetButtonDown("Prev")){
                SelectedPokemonIndex -= 1;

                if(SelectedPokemonIndex <= -1){
                    SelectedPokemonIndex = playerParty.Count - 1;
                }
            }

            if(SelectedPokemonIndex != prevSelection){
                summaryScreen.SetBasicDetails(playerParty[SelectedPokemonIndex]);
                summaryScreen.ShowPage(selectedPage);
            }
        }

        if(Input.GetButtonDown("Action")){
            if(selectedPage == 1 && !summaryScreen.InMoveSelection){
                summaryScreen.InMoveSelection = true;
            }
        } else if(Input.GetButtonDown("Back")){
            if(summaryScreen.InMoveSelection){
                summaryScreen.InMoveSelection = false;
            } else {
                gameController.StateMachine.Pop();
                return;
            }
        }

        summaryScreen.HandleUpdate();
    }

    public override void Exit(){
        summaryScreen.gameObject.SetActive(false);
    }
}
