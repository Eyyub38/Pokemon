using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Items/Create new pokeball")]
public class PokeballItem : ItemBase
{
    [SerializeField] float catchRateModifier = 1f;
    public float CatchRateModifier => catchRateModifier;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
    public override bool canUsedOutsideBattle => false;
}