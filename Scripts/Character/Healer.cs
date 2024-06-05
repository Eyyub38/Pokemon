using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour{
    public IEnumerator Heal(Transform player, Dialog dialog){
        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("Welcome to Pokemon Center. Do you want to take care your pokemons?",choices: new List<string>(){"Yes please","No thanks"} ,onChoiceSelected: (choiceIndex) => selectedChoice = choiceIndex);

        if(selectedChoice == 0){
            yield return Fader.i.FadeIn(0.5f);

            var playerParty = player.GetComponent<PokemonParty>();
            playerParty.Pokemons.ForEach(p => p.Heal());
            playerParty.PartyUpdated();

            yield return Fader.i.FadeOut(0.5f);
            yield return DialogManager.Instance.ShowDialogText($"You can take yoru pokemons back. Take care yourself.");

        } else if(selectedChoice == 1){
            yield return DialogManager.Instance.ShowDialogText($"Please take care your pokemons. If you need me I'm here for your need");
        }

    }
}
