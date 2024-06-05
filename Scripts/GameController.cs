using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System;
using GDEUtills.StateMachine;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    TrainerController trainer;

    public StateMachine<GameController> StateMachine {get; private set;} 

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    public static GameController Instance{ get; private set; }

    public PlayerController PlayerController => playerController;
    public Camera WorldCamera => worldCamera;
    public PartyScreen PartyScreen => partyScreen;
    
    public void Awake(){
        Instance = this;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ConditionsDB.Init();
        PokemonDB.Init();
        MoveDB.Init();
        ItemDB.Init();
        QuestDB.Init();
    }

    private void Start(){
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(FreeRoamState.i);

        battleSystem.OnBattleOver+=EndBattle;

        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () => {
            StateMachine.Push(DialogState.i);
        };
        DialogManager.Instance.OnDialogFinish += () => {
            StateMachine.Pop();
        };
    }

    public void PauseGame(bool pause){
        if(pause == true){
            StateMachine.Push(PauseState.i);
        } else {
            StateMachine.Pop();
        }
    }

    public void StartBattle(BattleTrigger trigger){
        BattleState.i.trigger = trigger;
        StateMachine.Push(BattleState.i);
    }

    public void StartTrainerBattle(TrainerController trainer){
        BattleState.i.trainer = trainer;
        StateMachine.Push(BattleState.i);
    }

    public void OnEnterTrainersView(TrainerController trainer){
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    void EndBattle(bool won){
        if(trainer != null && won == true){
            trainer.BattleLost();
            trainer = null;
        }
        partyScreen.SetPartyData();

        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        var playerParty = playerController.GetComponent<PokemonParty>();
        bool hasEvolutions = playerParty.CheckForEvolution();
        if(hasEvolutions){
            StartCoroutine(playerParty.RunEvolutions());
        } else {
            AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);
        }
    }

    private void Update(){
        StateMachine.Execute();
    }

    public void SetCurrentScene(SceneDetails currentScene){
        PrevScene = CurrentScene;
        CurrentScene = currentScene;
    }

    public IEnumerator MoveCamera(Vector2 moveOffSet, bool waitForFadeOut = false){
        yield return Fader.i.FadeIn(0.5f);

        worldCamera.transform.position += new Vector3(moveOffSet.x, moveOffSet.y);

        if(waitForFadeOut){
            yield return Fader.i.FadeOut(0.5f);
        } else {
            StartCoroutine(Fader.i.FadeOut(0.5f));
        }
    }

    private void OnGUI(){
        var style = new GUIStyle();
        style.fontSize = 24;
        
        GUILayout.Label("STATE STACK", style);
        foreach(var state in StateMachine.StateStack){
            GUILayout.Label(state.GetType().ToString(), style);
        }
    }
}
