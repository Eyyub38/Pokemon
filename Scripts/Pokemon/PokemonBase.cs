using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Pokemon",menuName ="Pokemon/Create new pokemon")]

public class PokemonBase : ScriptableObject{
    [SerializeField] string _name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite icon;
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;//Special Attack
    [SerializeField] int spDefense;
    [SerializeField] int speed;
    [SerializeField] int expYield;
    [SerializeField] int catchRate = 255;

    [SerializeField] GrowthRate growthRate;
    
    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> learnableByItems;

    [SerializeField] List<Evolution> evolutions;
    
    public static int MaxNumberOfMoves {get;set;} = 4;

    public string Name => _name;

    public Sprite BackSprite => backSprite;
    public Sprite FrontSprite => frontSprite;
    public Sprite Icon => icon;
    public int CatchRate => catchRate;
    public int ExpYield => expYield; 

    public PokemonType Type1 => type1;
    public PokemonType Type2 => type2;

    public string Description => description;

    public int MaxHP => maxHp;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;

    public GrowthRate GrowthRate => growthRate;

    public List<LearnableMove> LearnableMoves => learnableMoves;
    public List<MoveBase> LearnableByItems => learnableByItems;
    public List<Evolution> Evolutions => evolutions;
    
    public int GetExpForLevel(int level){
        if(growthRate == GrowthRate.Fast){
            return 4 * (level * level * level) / 5;
        } else if(growthRate == GrowthRate.MediumFast){
            return level * level * level;
        } else if(growthRate == GrowthRate.Erratic){
            if(level < 50){
                return (level * level * level) * (100 - level) / 50; 
            } else if(level >= 50 && level < 68){
                return (level * level * level) * (150 - level) / 100; 
            } else if(level >= 68 && level < 98){
                return (level * level * level) * ((1911 - (10 * level)) / 3) / 500;
            } else if(level >=98 ){
                return (level * level * level) * (160 - level) / 100; 
            } else {
                return -1;
            }
        } else if(growthRate == GrowthRate.Fluctuating){
            if(level < 15){
                return (level * level * level) * (level + 73) / 150; 
            } else if(level >= 15 && level < 36){
                return (level * level * level) * (level + 14) / 50; 
            } else if(level >= 36){
                return (level * level * level) * (level + 72) / 100;
            }
        } else if(growthRate == GrowthRate.Slow){
            return 5 * (level * level * level) / 4;
        } else if(growthRate == GrowthRate.MediumSlow){
            return (6 * level * level * level) / 5 - (15 * level * level) + (100 * level) - 140;
        }
        return -1;
}
}
[System.Serializable]
public class LearnableMove{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base => moveBase;
    public int Level => level;
}

[Serializable]
public class Evolution{
    [SerializeField] PokemonBase evolvesInto;
    [SerializeField] int requiredLevel;
    [SerializeField] EvolutionItem requiredItem;

    public PokemonBase EvolvesInto => evolvesInto;
    public int RequiredLevel => requiredLevel;
    public EvolutionItem RequiredItem => requiredItem;
}

public enum PokemonType{
    None, Normal, Fire, Water, Electric, Grass, Ice, Fight, Poison, Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy,
}

public enum GrowthRate{
    Erratic, Fast, MediumFast, MediumSlow, Slow, Fluctuating
}

public enum Stat{
    Attack, Defense, SpAttack, SpDefense, Speed,
    //These two are not actual stats,they're used to boost the moveAccuracy
    Accuracy, Evasion
}

public class TypeChart{
    public static float GetEffectiveness(PokemonType attackType,PokemonType defenseType){
        if(attackType==PokemonType.None || defenseType==PokemonType.None){
            return 1;
        }
        int row=(int)attackType - 1;
        int col=(int)defenseType - 1;

        return chart[row][col];
    }
    static float[][] chart={
        //                   Nor   Fir   Wat   Ele   Gra   Ice   Fig   Poi   Gro   Fly   Psy   Bug   Roc   Gho   Dra   Dar   Ste   Fai 
/*Normal*/  new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 0f,   1f,   1f,   0.5f, 1f },
/*Fire*/    new float[] {1f,   0.5f, 0.5f, 1f,   2f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   0.5f, 1f,   2f,   1f },
/*Water*/   new float[] {1f,   2f,   0.5f, 1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   1f,   1f },
/*Electric*/new float[] {1f,   1f,   2f,   0.5f, 0.5f, 1f,   1f,   1f,   0f,   2f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   1f },
/*Grass*/   new float[] {1f,   0.5f, 2f,   1f,   0.5f, 1f,   1f,   0.5f, 2f,   0.5f, 1f,   0.5f, 2f,   1f,   0.5f, 1f,   0.5f, 1f },
/*Ice*/     new float[] {1f,   0.5f, 0.5f, 1f,   2f,   0.5f, 1f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f },
/*Fighting*/new float[] {2f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f, 0.5f, 0.5f, 2f,   0f,   1f,   2f,   2f,   0.5f },
/*Poison*/  new float[] {1f,   1f,   1f,   1f,   2f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   0f,   2f },
/*Ground*/  new float[] {1f,   2f,   1f,   2f,   0.5f, 1f,   1f,   2f,   1f,   0f,   1f,   0.5f, 2f,   1f,   1f,   1f,   2f,   1f },
/*Flying*/  new float[] {1f,   1f,   1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   0.5f, 1f },
/*Psychic*/ new float[] {1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f,   0.5f, 1f,   1f,   1f,   1f,   0f,   0.5f, 1f },
/*Bug*/     new float[] {1f,   0.5f, 1f,   1f,   2f,   1f,   0.5f, 0.5f, 1f,   0.5f, 2f,   1f,   1f,   0.5f, 1f,   2f,   0.5f, 0.5f },
/*Rock*/    new float[] {1f,   2f,   1f,   1f,   1f,   2f,   0.5f, 1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   0.5f, 1f },
/*Ghost*/   new float[] {0f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f,   0.5f, 1f,   1f },
/*Dragon*/  new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 0f },
/*Dark*/    new float[] {1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f },
/*Steel*/   new float[] {1f,   0.5f, 0.5f, 0.5f, 1f,   2f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   0.5f, 2f },
/*Fairy*/   new float[] {1f,   0.5f, 1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   0.5f, 1f }
    };
}