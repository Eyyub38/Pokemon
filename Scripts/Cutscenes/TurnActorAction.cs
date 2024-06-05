using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnActorAction : CutsceneAction{
    [SerializeField] CutSceneActor actor;
    [SerializeField] FacingDirection direction;

    public override IEnumerator Play(){
        actor.GetCharacter().Animator.SetFacingDirection(direction);
        yield break;
    }
}