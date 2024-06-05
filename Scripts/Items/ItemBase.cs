using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject{
    [SerializeField] string _name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] float price;
    [SerializeField] bool isSellable;

    public virtual string Name => _name;
    public string Description => description;
    public Sprite Icon => icon;
    public float Price => price;
    public bool IsSellable => isSellable; 

    public virtual bool Use(Pokemon pokemon){
        return false;
    }

    public virtual bool IsReuseble => false;
    public virtual bool canUsedInBattle => true;
    public virtual bool canUsedOutsideBattle => true;
}
