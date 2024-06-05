using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class Fader : MonoBehaviour{
    public static Fader i{get;private set;}
    Image image;
    private void Awake(){
        image = GetComponent<Image>();
    }

    public IEnumerator FadeIn(float time){
        i = this;
        yield return image.DOFade( 1f, time).WaitForCompletion();
    }

    public IEnumerator FadeOut(float time){
        yield return image.DOFade( 0f, time);
    }
}
