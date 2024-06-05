using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GDE.GenericSelectionUI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MenuController : SelectionUI<TextSlot>{
    private void Start(){
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
}
