using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class EvolutionState : State<GameController>{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image pokemonImage;
    [SerializeField] AudioClip evolutionMusic;

    public static EvolutionState i {get;private set;}

    private void Awake(){
        i= this;
    }

    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution){
        var gameController = GameController.Instance;
        
        gameController.StateMachine.Push(this);
        evolutionUI.SetActive(true);

        AudioManager.i.PlayMusic(evolutionMusic);

        pokemonImage.sprite = pokemon.Base.FrontSprite;

        yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is evolving");

        var oldPokemon = pokemon.Base;
        pokemon.Evolve(evolution);
        pokemonImage.sprite = pokemon.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"{oldPokemon.Name} evolved into {pokemon.Base.Name}");
        evolutionUI.SetActive(false);

        gameController.PartyScreen.SetPartyData();
        AudioManager.i.PlayMusic(gameController.CurrentScene.SceneMusic, fade: true);

        gameController.StateMachine.Pop();
    }
}
