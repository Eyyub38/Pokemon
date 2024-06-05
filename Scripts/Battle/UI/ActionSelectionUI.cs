using UnityEngine;
using System.Linq;
using System.Collections;
using GDE.GenericSelectionUI;
using System.Collections.Generic;

public class ActionSelectionUI : SelectionUI<TextSlot>{
    private void Start(){
        SetSelectionSettings(SelectionType.Grid, 2);
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
}
