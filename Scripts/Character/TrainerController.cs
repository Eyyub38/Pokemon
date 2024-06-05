using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] string _name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] AudioClip trainerAppearsMusic;
    public string Name => _name;
    public Sprite Sprite => sprite;

    //State
    bool battleLost = false;


    Character character;

    private void Awake(){
        character = GetComponent<Character>();
    }

    public void Start(){
        SetFovRotation(character.Animator.DefaultDirection);
    }

    private void Update(){
        character.HandleUpdate();
    }

    public IEnumerator Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);

        if(!battleLost){
            AudioManager.i.PlayMusic(trainerAppearsMusic);
            
            yield return DialogManager.Instance.ShowDialog(dialog);
            GameController.Instance.StartTrainerBattle(this);
        } else {
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
        }
    }
    
    public void SetFovRotation(FacingDirection dir){
        float angle = 0f;
        if(dir==FacingDirection.Right){
            angle=90f;
        } else if(dir==FacingDirection.Up){
            angle=180f;
        } else if(dir==FacingDirection.Left){
            angle=270f;
        }

        fov.transform.eulerAngles = new Vector3(0f,0f,angle);
    }

    public void BattleLost(){
        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player){
        GameController.Instance.StateMachine.Push(CutsceneState.i);
        AudioManager.i.PlayMusic(trainerAppearsMusic);

        //Show exclamantaion
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        exclamation.SetActive(false);

        //Walk towards the player
        var diff=player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;

        moveVec = new Vector2(Mathf.Round(moveVec.x) , Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        //Show dialog
        yield return DialogManager.Instance.ShowDialog(dialog);
        
        GameController.Instance.StateMachine.Pop();
        GameController.Instance.StartTrainerBattle(this);
    }

    public object CaptureState(){
        return battleLost;
    }

    public void RestoreState(object state){
        battleLost = (bool)state;

        if(battleLost){
            fov.gameObject.SetActive(false);
        }
    }
}
