using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConditionsDB
{
    public static void Init(){
        foreach (var kvp in Conditions){
            var conditionId=kvp.Key;
            var condition=kvp.Value;

            condition.Id=conditionId;
        }
    }
    public static Dictionary<ConditionID,Condition> Conditions{get;set;} = new Dictionary<ConditionID, Condition>(){
        {ConditionID.psn,
        new Condition()
            {Name="Poison",
            StartMessage="has been poisoned.",
            OnAfterTurn= (Pokemon pokemon) =>{
                    pokemon.DecreaseHp(pokemon.MaxHp/8);
                    Debug.Log($"Poison damage applied to {pokemon.Base.Name}.");
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} took damage from poison.");
                }
            }
        },
        {ConditionID.brn,
            new Condition()
            {Name="Burn",
            StartMessage="has been burned.",
            OnAfterTurn= (Pokemon pokemon) =>{
                    pokemon.DecreaseHp(pokemon.MaxHp/16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} took damage from burning.");
                }
            }
        },
        {ConditionID.par,
        new Condition()
        {Name="Paralyzed",
        StartMessage="has been paralyzed.",
            OnBeforeMove=(Pokemon pokemon) => {
                if(Random.Range(1,5)==1){
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} cannot move because it is paralyzed.");
                    return false;
                }
                return true;
            }
            }
        },
        {ConditionID.frz,
        new Condition()
        {Name="Frozen",
        StartMessage="has been frozen.",
            OnBeforeMove=(Pokemon pokemon) => {
                if(Random.Range(1,5)==1){
                    pokemon.CureStatus();
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is not frozen anymore.");
                    return true;
                }
                return false;
            }
            }
        },
        {ConditionID.slp,
        new Condition()
        {Name="Sleep",
        StartMessage="has fallen asleep.",
            OnStart=(Pokemon pokemon)=>{
              //Sleep for 1-3 turns
              pokemon.StatusTime=Random.Range(1,3);
              Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");          
            },
            OnBeforeMove=(Pokemon pokemon) => {
                if(pokemon.StatusTime<=0){
                    pokemon.CureStatus();
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                    return true;
                }
                pokemon.StatusTime--;
                pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping");
                return false;
            }
            }
        },
        //Volatile Status Conditions
        {ConditionID.confusion,
        new Condition()
        {Name="Confusion",
        StartMessage="has been confused.",
            OnStart=(Pokemon pokemon)=>{
              //Confused for 1-4 turns
              pokemon.VolatileStatusTime=Random.Range(1,5);
              Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} moves");          
            },
            OnBeforeMove=(Pokemon pokemon) => {
                if(pokemon.VolatileStatusTime<=0){
                    pokemon.CureVolatileStatus();
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} kicked out confusion!");
                    return true;
                }
                pokemon.VolatileStatusTime--;

                //%50 chance to do move
                if(Random.Range(1,4)==1){
                    return true;
                }
                pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused.");                
                pokemon.DecreaseHp(pokemon.MaxHp/8);
                pokemon.StatusChanges.Enqueue($"It hurt itself due to confusion.");
                return false;
            }
            }
        }
    };

    public static float GetStatusBonus(Condition condition){
        if(condition == null){
            return 1f;
        } else if(condition.Id == ConditionID.frz || condition.Id == ConditionID.slp){
            return 2f;
        } else if (condition.Id == ConditionID.psn || condition.Id == ConditionID.brn || condition.Id == ConditionID.par){
            return 1.5f;
        }
        return 1f;
    }   
}

public enum ConditionID{
    none,
    psn,//Poison
    brn,//Burn
    slp,//Sleep
    par,//Paralyze
    frz,//Freeze
    confusion,//Confusion
}