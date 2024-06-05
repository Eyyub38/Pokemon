using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using GDE.GenericSelectionUI;
using System.Collections.Generic;

public class InventoryUI : SelectionUI<TextSlot>{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    [SerializeField] Text categoryText;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    List<ItemSlotUI> slotUIList;
    
    int selectedCategory = 0;
    const int itemsInViewPort = 4;

    Inventory inventory;
    RectTransform itemListRect;

    public ItemBase SelectedItem => inventory.GetItem(selectedItem, selectedCategory);
    public int SelectedCategory => selectedCategory;

    private void Awake(){
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start(){
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList(){
        //Clear all the existing items
        foreach (Transform child in itemList.transform){
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        //Atach all items to inventory
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory)){
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        SetItems(slotUIList.Select(s => s.GetComponent<TextSlot>()).ToList());
        UpdateSelectionInUI();
    }

    public override void HandleUpdate(){
        int prevCategory = selectedCategory;
        
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            ++selectedCategory;
        } else if(Input.GetKeyDown(KeyCode.LeftArrow)){
            --selectedCategory;
        }

        if(selectedCategory > Inventory.ItemCategories.Count -1){
            selectedCategory = 0;
        } else if(selectedCategory < 0){
            selectedCategory = Inventory.ItemCategories.Count - 1;
        }

        if(prevCategory != selectedCategory){
            ResetSelection();
            categoryText.text= Inventory.ItemCategories[selectedCategory];
            UpdateItemList();
        }

        base.HandleUpdate();
    }

    public override void UpdateSelectionInUI(){
        base.UpdateSelectionInUI();

        var slots = inventory.GetSlotsByCategory(selectedCategory);    
        if(slots.Count > 0){
            Debug.Log($"{selectedItem}");
            var item = slots[selectedItem].Item;

            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }

        HandleScrolling();
    }

    void HandleScrolling(){

        if(slotUIList.Count <= itemsInViewPort){
            return;
        }
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewPort/2, 0 ,selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemsInViewPort/2;
        upArrow.gameObject.SetActive(showUpArrow);
        
        bool showDownArrow = selectedItem + itemsInViewPort/2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }

    void ResetSelection(){
        selectedItem = 0;
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }
}
