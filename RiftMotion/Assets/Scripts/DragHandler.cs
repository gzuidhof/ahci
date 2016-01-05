using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using TalesFromTheRift;

public class DragHandler : MonoBehaviour, IDragHandler, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler {

    public CanvasKeyboard keyboard;
    private float duration;
    private bool onKey;
    
    // Use this for initialization
    void Start() {
        try {
            keyboard = GameObject.FindObjectsOfType<CanvasKeyboard>()[0];
        }
        catch {
            keyboard = null;
            Debug.Log("No keyboard"); //should not happen
        }
        duration = 0;
        onKey = false;
    
        
    }
    // Update is called once per frame
    void Update () {

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (onKey)
            duration += Time.deltaTime;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if(eventData.dragging && gameObject.name.Length == 1)
        {
            Debug.Log("Entering: " + gameObject.name);
            onKey = true;
            //AddCharacter();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onKey = true;
        /*
        if (gameObject.name.Length == 1) {
            AddCharacter();
            Debug.Log("OnDown: " + gameObject.name);
        }
        */
        
    }

    private void AddCharacter()
    {
        Debug.Log("DragAdding: " + gameObject.name[0]);
                
        keyboard.swyper.AddCharacter(gameObject.name[0], duration);
        duration = 0;
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
        onKey = false;
        AddCharacter();
        keyboard.swyper.EndOfInput();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(eventData.dragging && gameObject.CompareTag("Key"))
            {
            onKey = false;
            AddCharacter();

        }
    }
}
