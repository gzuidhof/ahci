using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using TalesFromTheRift;

public class DragHandler : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler {

    CanvasKeyboard keyboard;
    
    // Use this for initialization
    void Start() {
        try {
            keyboard = GameObject.FindObjectsOfType<CanvasKeyboard>()[0];
        }
        catch {
            keyboard = null;
            Debug.Log("No keyboard"); //should not happen
        }

        
    }
    // Update is called once per frame
    void Update () {

    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging");
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if(eventData.dragging)
        {
            
            AddCharacter();

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AddCharacter();
    }

    private void AddCharacter()
    {
        keyboard.SendKeyString(gameObject.name);
    }

    private void DraggingDone()
    {
        String input = keyboard.text;
        Debug.Log(input);
        keyboard.getSuggestion();
        keyboard.text = "";
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Dragging done!");
        DraggingDone();
    }
}
