using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterSelectorUI : MonoBehaviour{
    [SerializeField] Text countText;
    [SerializeField] Text priceText;

    bool selected;
    int currCount;
    int maxCount;

    float pricePerUnit;

    public IEnumerator ShowSelector(int maxCount, float pricePerUnit, Action<int> onCountSelected){
        this.maxCount = maxCount;
        this.pricePerUnit = pricePerUnit;

        selected = false;
        currCount = 1;

        gameObject.SetActive(true);
        SetValues();

        yield return new WaitUntil(() => selected == true);

        onCountSelected?.Invoke(currCount);
        gameObject.SetActive(false);
    }

    void SetValues(){
        countText.text = "x" + currCount;
        priceText.text ="$" + pricePerUnit * currCount;
    }

    private void Update(){
        var prevCount = currCount;
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            ++currCount;
        } else if(Input.GetKeyDown(KeyCode.DownArrow)){
            --currCount;
        }

        currCount = Mathf.Clamp(currCount, 1, maxCount);
        if(currCount != prevCount){
           SetValues(); 
        }

        if(Input.GetKeyDown(KeyCode.Return)){
            selected = true;
        }
    }
}
