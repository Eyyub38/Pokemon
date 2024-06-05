using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GlobalSettings : MonoBehaviour{
    [SerializeField] Color highlitghtedColor;
    [SerializeField] Color backgroundHighlightedColor;

    public Color HighlightedColor => highlitghtedColor;
    public Color BackgroundHighlightedColor => backgroundHighlightedColor;

    public static GlobalSettings i { get; private set;}

    public void Awake(){
        i = this;
    }
}