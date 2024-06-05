using UnityEngine;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class BattleState : State<GameController>{
    [SerializeField] BattleSystem battleSystem;

    public static BattleState i{get; private set;}

    GameController gameController;
    
    //Input
    public BattleTrigger trigger {get; set;}
    public TrainerController trainer {get; set;}

    public BattleSystem BattleSystem => battleSystem;

    private void Awake(){
        i = this;
    }

    public override void Enter(GameController owner){
        gameController = owner;

        battleSystem.gameObject.SetActive(true);
        gameController.WorldCamera.gameObject.SetActive(false);

        var playerParty =gameController.PlayerController.GetComponent<PokemonParty>();
        if(trainer == null){
            var wildPokemon=gameController.CurrentScene.GetComponent<MapArea>().GetRandomwildPokemon(trigger);
            var wildPokemonCopy=new Pokemon( wildPokemon.Base, wildPokemon.Level );
            battleSystem.StartBattle( playerParty, wildPokemonCopy, trigger);
        } else {
            var trainerParty =trainer.GetComponent<PokemonParty>();
            battleSystem.StartTrainerBattle(playerParty,trainerParty);
        }

        battleSystem.OnBattleOver += EndBattle;
    }

    public override void Execute(){
        battleSystem.HandleUpdate();
    }

    void EndBattle(bool won){
        if(trainer != null && won == true){
            trainer.BattleLost();
            trainer = null;
        }

        gameController.StateMachine.Pop();
    }

    public override void Exit(){
        battleSystem.gameObject.SetActive(false);
        gameController.WorldCamera.gameObject.SetActive(true);

        battleSystem.OnBattleOver -= EndBattle;
    }
}
