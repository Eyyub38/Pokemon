using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PartyState : State<GameController>{
    [SerializeField] PartyScreen partyScreen;
    GameController gameController;
    PokemonParty playerParty;

    bool isSwitchingPosition;
    int selectedIndexForSwitch = 0;

    public Pokemon SelectedPokemon{get; private set;}
    public static PartyState i {get; private set;}

    private void Awake(){
        i = this;
    }

    private void Start(){
        playerParty = PlayerController.i.GetComponent<PokemonParty>();
    }

    public override void Enter(GameController owner){
        gameController = owner;

        SelectedPokemon = null;
        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;
    }

    void OnPokemonSelected(int selection){
        SelectedPokemon = partyScreen.SelectedMember;

        StartCoroutine(PokemonSelectedAction(selection));
    }

    IEnumerator PokemonSelectedAction(int selectedPokemonIndex){
        var prevState = gameController.StateMachine.GetPreviousState();

        if(prevState == InventoryState.i){
            //Use Item
            StartCoroutine(GoToUseItemState());

        } else if(prevState == BattleState.i){
            var battleState = prevState as BattleState;

            DynamicMenuState.i.MenuItems = new List<string>() {"Shift", "Summary", "Cancel"};
            yield return gameController.StateMachine.PushAndWait(DynamicMenuState.i);

            if(DynamicMenuState.i.SelectedItem == 0){
                if(SelectedPokemon.HP<=0){
                    partyScreen.SetMessageText("You can't send out a fainted pokemon");
                    yield break;
                }
                if(SelectedPokemon == battleState.BattleSystem.PlayerUnit.Pokemon){
                    partyScreen.SetMessageText("Did you forget the pokemon you're fighting right now?");
                    yield break;
                }

            gameController.StateMachine.Pop();

            } else if (DynamicMenuState.i.SelectedItem == 1){
                SummaryState.i.SelectedPokemonIndex = selectedPokemonIndex;
                yield return gameController.StateMachine.PushAndWait(SummaryState.i);
            } else {
                yield break;
            }

        } else {
            if(isSwitchingPosition){
                if(selectedIndexForSwitch == selectedPokemonIndex){
                    partyScreen.SetMessageText("You didn't change position of this pokemon!");
                    yield break;
                }
                isSwitchingPosition = false;

                var tempPokemon =  playerParty.Pokemons[selectedIndexForSwitch];
                playerParty.Pokemons[selectedIndexForSwitch] = playerParty.Pokemons[selectedPokemonIndex];
                playerParty.Pokemons[selectedPokemonIndex] = tempPokemon;
                playerParty.PartyUpdated();

                yield break;
            }
            DynamicMenuState.i.MenuItems = new List<string>() {"Summary", "Switch", "Cancel"};
            yield return gameController.StateMachine.PushAndWait(DynamicMenuState.i);
            if(DynamicMenuState.i.SelectedItem == 0){
                SummaryState.i.SelectedPokemonIndex = selectedPokemonIndex;
                yield return gameController.StateMachine.PushAndWait(SummaryState.i);
                
            } else if (DynamicMenuState.i.SelectedItem == 1){
                //Switch Pokemon Location
                isSwitchingPosition = true;
                selectedIndexForSwitch = selectedPokemonIndex;
                partyScreen.SetMessageText($"Choose a pokemon switch position with.");

            } else {
                yield break;
            }
        }
    }

    void OnBack(){
        SelectedPokemon = null;
        
        var prevState = gameController.StateMachine.GetPreviousState();
        if(prevState == BattleState.i){
            var battleState = prevState as BattleState;

            if(battleState.BattleSystem.PlayerUnit.Pokemon.HP <= 0){
                partyScreen.SetMessageText( "You have to change your pokemon to continue" );
                return;
            }
        }
        gameController.StateMachine.Pop();
    }

    public override void Execute(){
        partyScreen.HandleUpdate();
    }

    public override void Exit(){
        partyScreen.gameObject.SetActive(false);
        
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
    }

    IEnumerator GoToUseItemState(){
        yield return gameController.StateMachine.PushAndWait(UseItemState.i);
        gameController.StateMachine.Pop();
    }
}
