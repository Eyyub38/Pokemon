using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisableObjectAction : CutsceneAction{
    [SerializeField] GameObject gameObject;

    public override IEnumerator Play(){
        gameObject.SetActive(false);
        yield break;
    }
}