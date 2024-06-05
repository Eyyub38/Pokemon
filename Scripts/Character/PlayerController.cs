using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable{
    [SerializeField] string _name;
    [SerializeField] Sprite sprite;
    private Vector2 input;
    private Character character;

    public static PlayerController i{ get; private set;}

    private void Awake(){
        i = this;
        character = GetComponent<Character>();
    }

    public void HandleUpdate(){
        if(!character.IsMoving){

            input.x=Input.GetAxisRaw("Horizontal");
            input.y=Input.GetAxisRaw("Vertical");

            if(input.x != 0) input.y=0;

            if(input != Vector2.zero){
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();
        
        if(Input.GetKeyDown(KeyCode.F)){
            StartCoroutine(Interact());
        }
    }

    IPlayerTriggerable currentlyInTrigger;

    private void OnMoveOver(){
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3( 0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);
        IPlayerTriggerable triggerable = null;

        foreach(var collider in colliders){
            triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable != null){
                if(triggerable == currentlyInTrigger && !triggerable.TriggerRepeatdly){
                    break;
                }

                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break;
            }
        }

        if(colliders.Count() == 0 || triggerable != currentlyInTrigger){
            currentlyInTrigger = null;
        }
    }

    public IEnumerator Interact(){
        var facingDir=new Vector3(character.Animator.MoveX,character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        Debug.DrawLine( transform.position, interactPos, Color.blue, 0.5f);
        var collider = Physics2D.OverlapCircle( interactPos, 0.3f, GameLayers.i.InteractableLayer | GameLayers.i.WaterLayer);

        if(collider != null){
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    public object CaptureState(){
        var saveData = new PlayerSaveData(){
            position = new float[] { transform.position.x, transform.position.y},
            pokemons = GetComponent<PokemonParty>().Pokemons.Select(p => p.GetSaveData()).ToList()
        };

        return saveData;
    }

    public void RestoreState(object state){
        var saveData = ( PlayerSaveData ) state;

        var position = saveData.position; 
        transform.position = new Vector3( position[0], position[1] );

        GetComponent<PokemonParty>().Pokemons = saveData.pokemons.Select(s => new Pokemon(s)).ToList();
    }

    public string Name => _name;
    
    public Sprite Sprite => sprite;

    public Character Character => character;
}

[Serializable]
public class PlayerSaveData{
    public float[] position;
    public List<PokemonSaveData> pokemons;

}