using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Items/Create new TM or HM")]

public class TmItem : ItemBase{
    [SerializeField] MoveBase move;
    [SerializeField] bool isHM;

    public MoveBase Move => move;
    public bool IsHM => isHM;

    public override string Name => base.Name + $": {move.Name}";
    public override bool canUsedInBattle => false;
    public override bool IsReuseble => isHM;

    public override bool Use(Pokemon pokemon)
    {
        //Learning move is handled from inventory UI, if it was learned then return true 
        return pokemon.HasMove(move);
    }

    public bool CanBeTaught(Pokemon pokemon){
        return pokemon.Base.LearnableByItems.Contains(move);
    }

}