using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text messageText;
    [SerializeField] HpBar hpBar;

    Pokemon _pokemon;

    public void Init(Pokemon pokemon){
        _pokemon=pokemon;
        UpdateData();
        SetMessage("");

        _pokemon.OnHPChanged += UpdateData;
    }

    public void UpdateData(){
        nameText.text= _pokemon.Base.Name;
        levelText.text="Lvl" + _pokemon.Level;
        hpBar.SetHP((float) _pokemon.HP / _pokemon.MaxHp);
    }
    
    public void SetSelected(bool selected){
        if(selected){
            nameText.color=GlobalSettings.i.HighlightedColor;
        } else {
            nameText.color =Color.black;
        }
    }

    public void SetMessage(string message){
        messageText.text = message;
    }
}
