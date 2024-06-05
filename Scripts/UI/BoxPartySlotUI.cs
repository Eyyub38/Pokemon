using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BoxPartySlotUI : MonoBehaviour{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Image icon;

    public void SetData(Pokemon pokemon){
        nameText.text = pokemon.Base.name;
        levelText.text = "Lvl. " + pokemon.Level;
        icon.sprite = pokemon.Base.Icon;
        icon.color = new Color( 255, 255, 255, 100);
    }

    public void ClearData(){
        nameText.text = "";
        levelText.text = "";
        icon.sprite = null;
        icon.color = new Color( 255, 255, 255, 0);
    }
}