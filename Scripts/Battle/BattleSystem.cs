using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.Hardware;
using Unity.VisualScripting;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public enum BattleAction{ Move, SwitchPokemon, UseItem, Run}
public enum BattleTrigger{ LongGrass, Water}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] MoveToForgetSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;
    
    //Music
    [Header("Wild Battle Musics")]
    [SerializeField] AudioClip wildBattleMusic;
    [SerializeField] AudioClip wildBattleWonMusic;
    [Header("Trainer Battle Musics")]
    [SerializeField] AudioClip trainerBattleMusic;
    [SerializeField] AudioClip trainerBattleWonMusic;

    [Header("Background")]
    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite grassBackground;
    [SerializeField] Sprite waterBackground;

    public event Action<bool> OnBattleOver;

    BattleTrigger battleTrigger;    
    PlayerController player;
    
    public TrainerController Trainer{get; private set;}
    public PokemonParty PlayerParty{get; private set;}
    public PokemonParty TrainerParty{get; private set;}
    public Pokemon WildPokemon{get; private set;}
    public bool IsTrainerBattle{get; private set;} = false;
    public StateMachine<BattleSystem> StateMachine{get; private set;}
    public int SelectedMove{get; set;}
    public BattleAction SelectedAction{get; set;}
    public Pokemon SelectedPokemon{get; set;}
    public ItemBase SelectedItem{get; set;}
    public bool IsBattleOver{get; private set;}
    public int EscapeAttemps{get; set;}

    public BattleDialogBox DialogBox => dialogBox;
    public BattleUnit PlayerUnit => playerUnit;
    public BattleUnit EnemyUnit => enemyUnit;
    public PartyScreen PartyScreen => partyScreen;
    public AudioClip TrainerBattleWonMusic => trainerBattleWonMusic;
    public AudioClip WildBattleWonMusic => wildBattleWonMusic;

    public void StartBattle(PokemonParty playerParty,Pokemon wildPokemon, BattleTrigger trigger = BattleTrigger.LongGrass){
        this.PlayerParty = playerParty;
        this.WildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();
        IsTrainerBattle=false;
        AudioManager.i.PlayMusic(wildBattleMusic);


        battleTrigger = trigger;

        StartCoroutine(SetupBattle());
    }
    
    public void StartTrainerBattle(PokemonParty playerParty,PokemonParty trainerParty, BattleTrigger trigger = BattleTrigger.LongGrass){
        this.PlayerParty = playerParty;
        this.TrainerParty = trainerParty;

        IsTrainerBattle=true;

        player = playerParty.GetComponent<PlayerController>();
        Trainer = trainerParty.GetComponent<TrainerController>();

        battleTrigger = trigger;

        AudioManager.i.PlayMusic(trainerBattleMusic);

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle(){
        StateMachine = new StateMachine<BattleSystem>(this);

        playerUnit.Clear();
        enemyUnit.Clear();

        backgroundImage.sprite = (battleTrigger == BattleTrigger.LongGrass)? grassBackground : waterBackground;

        if(!IsTrainerBattle){
            //wild Pokemon Battle
            playerUnit.Setup(PlayerParty.GetHealthyPokemon());
            enemyUnit.Setup(WildPokemon);
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

            yield return StartCoroutine(dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appered!"));
        
        } else {
            //Trainer Battle

            //Show trainer and player Images
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite=playerImage.sprite;
            trainerImage.sprite=trainerImage.sprite;

            yield return dialogBox.TypeDialog($"{Trainer.Name} is challange to you");

            //Send out first pokemon of the trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);

            var enemyPokemon=TrainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);

            yield return dialogBox.TypeDialog($"{Trainer.Name} send out {enemyPokemon.Base.Name}");
            //Send out first pokemon of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);

            var playerPokemon=PlayerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"{playerPokemon.Base.Name}! I choose you.");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }

        IsBattleOver = false;
        EscapeAttemps = 0;
        partyScreen.Init();
        
        StateMachine.ChangeState(ActionSelectionState.i);
    }

    public void BattleOver(bool won){
        IsBattleOver = true;
        PlayerParty.Pokemons.ForEach(p=>p.OnBattleOver());
        OnBattleOver(won);
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
    }

    public void HandleUpdate(){
        StateMachine.Execute();
    }

    public IEnumerator SwitchPokemon(Pokemon newPokemon){
        if( playerUnit.Pokemon.HP > 0 ){
            yield return dialogBox.TypeDialog( $"Come back {playerUnit.Pokemon.Base.Name}. Thank you." );
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup( newPokemon );
        dialogBox.SetMoveNames( newPokemon.Moves );
        yield return dialogBox.TypeDialog( $"{ newPokemon.Base.Name } your turn." );
    }

    public IEnumerator SendNextTrainerPokemon(){
        var nextPokemon = TrainerParty.GetHealthyPokemon();
        enemyUnit.Setup( nextPokemon );
        yield return dialogBox.TypeDialog( $"{ Trainer.Name } send out { nextPokemon.Base.Name }!" );
    }

    public IEnumerator ThrowPokeball(PokeballItem pokeballItem){
        if(IsTrainerBattle){
            yield return dialogBox.TypeDialog($"{Trainer.Name}: Did you try steal my pokemon? You idiot.");
            yield break;
        }

        yield return dialogBox.TypeDialog($"Go {pokeballItem.Name.FirstCharacterToUpper()}!");

        var pokeballObj =Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2,0), Quaternion.identity);
        var pokeball=pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeballItem.Icon;

        //Animations
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0,2), 2f, 1, 1f).WaitForCompletion();
        //TODO: While catching animasion pokemons color is become red and after then it capture by pokeball
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y-2f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon ,pokeballItem);

        for( int i=0; i < Mathf.Min( shakeCount, 3); i++){
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3( 0, 0, 10f), (0.8f)).WaitForCompletion();
        }

        if(shakeCount == 4){
            //Pokemon is cought
            yield return dialogBox.TypeDialog($"You caught {enemyUnit.Pokemon.Base.Name}");
            yield return pokeball.DOFade( 0, 1.5f).WaitForCompletion();

            Destroy(pokeball.gameObject);
            BattleOver(true);
            PlayerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} has been added your party");
        } else {
            //Pokemon broke out
            yield return new WaitForSeconds(0.5f);
            pokeball.DOFade(0,0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();
            
            if(shakeCount < 2){
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} broke free");
            } else {
                yield return dialogBox.TypeDialog($"That's close! Try again.");
            }

            Destroy(pokeball.gameObject);
        }
    }
    int TryToCatchPokemon(Pokemon pokemon, PokeballItem pokeballItem){

        float rate_a=( 3 * pokemon.MaxHp - 2 ) * pokemon.Base.CatchRate * pokeballItem.CatchRateModifier *ConditionsDB.GetStatusBonus(pokemon.Status) / ( 3 * pokemon.MaxHp);
        if( rate_a >= 255 ){
            return 4;
        }
        float rate_b = 1048560 / Mathf.Sqrt( Mathf.Sqrt( 16711680 / rate_a ));

        int shakeCount = 0;
        while (shakeCount < 4){
            if( UnityEngine.Random.Range(0,65535) >= rate_b){
                break;
            }
            ++shakeCount;
        }

        return shakeCount;
    }
}
