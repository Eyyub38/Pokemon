using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Items/Crate new recovery item")]
public class RecoveryItems : ItemBase{
    //HP Items
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;
    
    //PP Items
    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;

    //Status Cure Items
    [Header("Status Cure")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    //Revive ==> Kaldıracağım ileride bunu
    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Pokemon pokemon)
    {
        //Revive
        if(revive || maxRevive){
            if(pokemon.HP > 0){
                return false;
            }
            if(revive){
                pokemon.IncreaseHp(pokemon.MaxHp / 2);
            } else if(maxRevive){
                pokemon.IncreaseHp(pokemon.MaxHp);
            }

            pokemon.CureStatus();

            return true;
        }

        //No other items can be used on fainted pokemon
        if(pokemon.HP == 0){
            return false;
        }

        //Poison
        if(restoreMaxHP || hpAmount > 0){
            if(pokemon.HP == pokemon.MaxHp){
                return false;
            }
            if (restoreMaxHP){
                pokemon.IncreaseHp(pokemon.MaxHp);
            } else {
                pokemon.IncreaseHp(hpAmount);
            }
        }

        //Recover status
        if(recoverAllStatus || status != ConditionID.none){
            if(pokemon.Status == null && pokemon.VolatileStatus == null){
                return false;
            }

            if(recoverAllStatus){
                pokemon.CureStatus();
                pokemon.CureVolatileStatus();
            } else {
                if(pokemon.Status.Id == status){
                    pokemon.CureStatus();
                } else if(pokemon.VolatileStatus.Id == status){
                    pokemon.CureVolatileStatus();
                } else {
                    return false;
                }
            }
        }

        //Restore PP
        if(restoreMaxPP){
            pokemon.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
        } else if(ppAmount > 0){
            pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
        }

        return true;
    }
}