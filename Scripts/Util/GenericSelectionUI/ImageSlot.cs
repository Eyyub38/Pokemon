using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ImageSlot : MonoBehaviour, ISelectableItem{
    Image backgroundImage;
    Color originalColor;

    private void Awake(){
        backgroundImage = GetComponent<Image>();
    }
    
    public void OnSelectionChange(bool selected){
        backgroundImage.color = (selected)? GlobalSettings.i.BackgroundHighlightedColor : originalColor;
    }

    public void Init(){
        originalColor = backgroundImage.color;
    }

    public void Clear(){
        backgroundImage.color = originalColor;
    }
}