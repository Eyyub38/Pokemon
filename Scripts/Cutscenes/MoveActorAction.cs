using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MoveActorAction : CutsceneAction{
    [SerializeField] CutSceneActor actor;
    [SerializeField] List<Vector2> movePatterns;


    public override IEnumerator Play(){
        var character = actor.GetCharacter();

        foreach (var moveVec in movePatterns){
            yield return character.Move(moveVec, checkCollisions: false);
        }
    }
}

[System.Serializable]
public class CutSceneActor{
    [SerializeField] bool isPlayer;
    [SerializeField] Character character;

    public Character GetCharacter() => (isPlayer)? PlayerController.i.Character : character;
}