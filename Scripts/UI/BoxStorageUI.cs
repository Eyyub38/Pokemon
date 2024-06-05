using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BoxStorageUI : MonoBehaviour{
    [SerializeField] Image icon;

    public void SetData(Pokemon pokemon){
        icon.sprite = pokemon.Base.Icon;
        icon.color = new Color( 255, 255, 255, 100);
    }

    public void ClearData(){
        icon.sprite = null;
        icon.color = new Color( 255, 255, 255, 0);
    }
}