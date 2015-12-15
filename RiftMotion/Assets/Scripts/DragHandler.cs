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
        
        if(eventData.dragging && gameObject.name.Length == 1)
        {
            Debug.Log("Entering: " + gameObject.name);
            AddCharacter();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameObject.name.Length == 1) {
            AddCharacter();
            Debug.Log("OnDown: " + gameObject.name);
        }
        
    }

    private void AddCharacter()
    {
        Debug.Log("DragAdding: " + gameObject.name[0]);
                
        keyboard.swyper.AddCharacter(gameObject.name[0]);
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
        keyboard.swyper.EndOfInput();
    }
}
