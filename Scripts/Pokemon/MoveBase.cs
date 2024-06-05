using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Move",menuName ="Pokemon/Create a Pokemon Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string _name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHit;
    [SerializeField] int pp;
    [SerializeField] int priority;
    [SerializeField] List<SeconaryEffects> secondaries;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;
    [SerializeField] AudioClip sound;

    public string Name => _name;
    public string Description => description;
    public PokemonType Type => type;
    public int Power => power;
    public List<SeconaryEffects> Secondaries => secondaries;
    public int Accuracy => accuracy;
    public bool AlwaysHit => alwaysHit;
    public int PP => pp;
    public int Priority => priority;
    public MoveCategory Category => category;
    public MoveEffects Effects => effects;
    public MoveTarget Target => target;
    public AudioClip Sound => sound;
}
[System.Serializable]
public class MoveEffects{
    [SerializeField] List<StatBoosts> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID voltileStatus;

    public List<StatBoosts> Boosts => boosts;
    public ConditionID Status => status;
    public ConditionID VoltileStatus => voltileStatus;

}

[System.Serializable]
public class SeconaryEffects : MoveEffects{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance => chance;
    public MoveTarget Target => target;
}

[System.Serializable]
public class StatBoosts{
    public Stat stat;
    public int boost;
}

public enum MoveCategory{
    Physical,
    Special,
    Status
}

public enum MoveTarget{
    Foe,
    Self
}
