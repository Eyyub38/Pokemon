using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextSlot : MonoBehaviour, ISelectableItem{
    [SerializeField] Text text;
    Color originalColor;
    
    public void OnSelectionChange(bool selected){
        text.color = (selected)? GlobalSettings.i.HighlightedColor : originalColor;
    }

    public void SetText(string textString){
        text.text = textString;
    }

    public void Init(){
        originalColor = text.color;
    }

    public void Clear(){
        text.color = originalColor;
    }
}
