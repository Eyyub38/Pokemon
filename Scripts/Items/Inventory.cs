using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Net.WebSockets;
using System.Collections.Generic;

public enum ItemCategory{Items, Pokeballs, TMs}

public class Inventory : MonoBehaviour, ISavable{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> pokeballSlots;
    [SerializeField] List<ItemSlot> tmSlots;

    List<List<ItemSlot>> allSlots;
    public static List<string> ItemCategories {get;set;} = new List<string>(){
        "ITEMS","POKEBALLS","TMs & HMs"
    };

    private void Awake(){
        allSlots = new List<List<ItemSlot>>(){slots, pokeballSlots, tmSlots};
    }

    public event Action OnUpdated;

    public static Inventory GetInventory(){
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
        
    }

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex){
        return allSlots[categoryIndex];
    }

    public ItemBase UseItem(int indexItem,Pokemon selectedPokemon, int selectedCategory){
        var item = GetItem(indexItem,selectedCategory);

        return UseItem(item, selectedPokemon);
    }
    
    public ItemBase UseItem(ItemBase item,Pokemon selectedPokemon){
        bool itemUsed = item.Use(selectedPokemon);

        if(itemUsed){
            if(!item.IsReuseble){
                RemoveItem(item);
            }
            return item;
        }
        return null;
    }

    public void RemoveItem(ItemBase item, int countToRemove = 1){
        int selectedCategory = (int)GetCategoryFromItem(item);
        var currSlots = GetSlotsByCategory(selectedCategory);
        
        var itemSlot = currSlots.First(slot => slot.Item == item);
        itemSlot.Count -= countToRemove;
        
        if(itemSlot.Count == 0){
            currSlots.Remove(itemSlot);
        }

        OnUpdated?.Invoke();
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex){
        var currSlots = GetSlotsByCategory(categoryIndex);
        return currSlots[itemIndex].Item;
    }

    public void AddItem(ItemBase item, int count = 1){
        int category = (int)GetCategoryFromItem(item);
        var currSlots = GetSlotsByCategory(category);

        var itemSlot = currSlots.FirstOrDefault(slot => slot.Item == item);

        if(itemSlot != null){
            itemSlot.Count += count;
        } else {
            currSlots.Add(new ItemSlot(){
                Item = item,
                Count = count
            });
        }

        OnUpdated?.Invoke();
    }

    ItemCategory GetCategoryFromItem(ItemBase item){
        if(item is RecoveryItems || item is EvolutionItem){
            return ItemCategory.Items;
        } else if(item is PokeballItem || item is EvolutionItem){
            return ItemCategory.Pokeballs;
        } else {
            return ItemCategory.TMs;
        }
    }

    //Save System
    public object CaptureState(){
        Debug.Log($"Problem yok gibi");
        var saveData = new InventorySaveData(){
            items = slots.Select(i => i.GetSaveData()).ToList(),
            pokeballs = pokeballSlots.Select(p => p.GetSaveData()).ToList(),
            tms = tmSlots.Select(t => t.GetSaveData()).ToList()
        };

        return saveData;
    }

    public int GetItemCount(ItemBase item){
        int category = (int)GetCategoryFromItem(item);
        var currSlots = GetSlotsByCategory(category);
        var itemSlot = currSlots.FirstOrDefault(slot => slot.Item == item);

        if(itemSlot != null){
            return itemSlot.Count;
        } else {
            return 0;
        }
    }

    public void RestoreState(object state){
        var saveData = state as InventorySaveData;

        slots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        pokeballSlots = saveData.pokeballs.Select(p => new ItemSlot(p)).ToList();
        tmSlots = saveData.tms.Select(t => new ItemSlot(t)).ToList();

        allSlots = new List<List<ItemSlot>>(){slots, pokeballSlots, tmSlots};

        OnUpdated?.Invoke();
    }

    public bool HasItem(ItemBase item){
        var category = (int)GetCategoryFromItem(item);
        var currSlots = GetSlotsByCategory(category);

        return currSlots.Exists(slot => slot.Item == item);

    }
}

[Serializable]
public class ItemSlot{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemSlot(){}
    
    public ItemSlot(ItemSaveData saveData){
        item = ItemDB.GetObjectByName(saveData.name);
        count = saveData.count;
    }

    public int Count{
        get => count;
        set => count = value;
    }

    public ItemBase Item{
        get => item;
        set => item = value;
    }

    public ItemSaveData GetSaveData(){
        var saveData = new ItemSaveData(){
            name = item.name,
            count = count
        };

        return saveData;
    }
}

[Serializable]
public class ItemSaveData{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData{
    public List<ItemSaveData> items;
    public List<ItemSaveData> pokeballs;
    public List<ItemSaveData> tms;
}