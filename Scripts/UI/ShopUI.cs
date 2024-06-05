using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopUI : MonoBehaviour{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    
    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    List<ItemBase> availableItems;
    List<ItemSlotUI> slotUIList;

    Action<ItemBase> onItemSelected;
    Action onBack;

    int selectedItem;
    const int itemsInViewPort = 5;
    RectTransform itemListRect;

    private void Awake(){
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void Show(List<ItemBase> availableItems, Action<ItemBase> onItemSelected, Action onBack){
        this.availableItems = availableItems;
        this.onItemSelected = onItemSelected;
        this.onBack = onBack;

        gameObject.SetActive(true);
        UpdateItemList();
    }

    public void Close(){
        gameObject.SetActive(false);
    }

    void UpdateItemList(){
        //Clear all the existing items
        foreach (Transform child in itemList.transform){
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        //Atach all items to inventory
        foreach (var item in availableItems){
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetNameAndPrice(item);

            slotUIList.Add(slotUIObj);
        }
        UpdateItemSelection();
    }

    private void UpdateItemSelection(){
        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);
        
        for (int i = 0; i < slotUIList.Count; i++){
            if(i == selectedItem){
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            } else {
                slotUIList[i].NameText.color = Color.black;
            }
        }
        
        if(availableItems.Count > 0){
            var item = availableItems[selectedItem];

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

    public void HandleUpdate(){
        var prevSelection = selectedItem;
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            ++selectedItem;
        } else if(Input.GetKeyDown(KeyCode.UpArrow)){
            --selectedItem;
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);

        if(selectedItem != prevSelection){
            UpdateItemSelection();
        }

        if(Input.GetKeyDown(KeyCode.Return)){
            onItemSelected?.Invoke(availableItems[selectedItem]);
        } else if(Input.GetKeyDown(KeyCode.Escape)){
            onBack?.Invoke();
        }
    }
}
