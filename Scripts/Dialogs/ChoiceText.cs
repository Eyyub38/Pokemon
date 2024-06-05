using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceText : MonoBehaviour{
    Text text;

    public Text TextField => text;
    public void Awake(){
        text = GetComponent<Text>();
    }

    public void SetSelected(bool selected){
        text.color = (selected)? GlobalSettings.i.HighlightedColor : Color.black;
    }
}
