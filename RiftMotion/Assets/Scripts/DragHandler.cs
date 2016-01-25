using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using TalesFromTheRift;

public class DragHandler : MonoBehaviour, IDragHandler, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler {

    public CanvasKeyboard keyboard;
    private float duration;
    private float starttime;
    private bool onKey;
    private char currentobject;
    
    // Use this for initialization
    void Start() {
        try {
            keyboard = GameObject.FindObjectsOfType<CanvasKeyboard>()[0];
        }
        catch {
            keyboard = null;
            Debug.Log("No keyboard"); //should not happen
        }
        starttime = 0;
        duration = 0;
        onKey = false;
    
        
    }
    // Update is called once per frame
    void Update () {
     //   if (onKey)
       // {
       //     duration += Time.deltaTime;
       // }
    }

    public void OnDrag(PointerEventData eventData)
    {
       
       
      //  Debug.Log(currentobject + duration);
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if(eventData.dragging && gameObject.name.Length == 1)
        {
            Debug.Log("Entering: " + gameObject.name);
           
                currentobject = gameObject.name[0];
                Debug.Log("HIER"+currentobject);
          
            //AddCharacter();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("CLICK! " + gameObject.name[0]);
        onKey = true;
        starttime = Time.time;
        /*
        if (gameObject.name.Length == 1) {
            AddCharacter();
            Debug.Log("OnDown: " + gameObject.name);
        }
        */

    }

    private void AddCharacter()
    {
        Debug.Log("DragAdding: " + duration+ gameObject.name[0]);
        keyboard.swyper.AddCharacter(gameObject.name[0], duration);
        
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
        Debug.Log("LOS!" + gameObject.name);
 
        onKey = false;
        Debug.Log(Time.time);
        duration = Time.time - starttime;
        starttime = Time.time;
        //AddCharacter(); //deze weggehaald omdat hij als laatste character perse het eerste character wil invullen. Iets toevoegen als current object waarin het laatste geenterde character zit werkt ook neit want dat pst hij gewoon aan...
        keyboard.swyper.EndOfInput();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(eventData.dragging && gameObject.CompareTag("Key"))
        {
            Debug.Log("TIJD:"+Time.time);
            Debug.Log("TIJD2:" + starttime);//alleen eerste keer heeft starttime blijkbaar een waarde die niet 0 is...
            Debug.Log("VERSCHIL:" + (Time.time - starttime));
            duration = Time.time - starttime;
            AddCharacter();
            starttime = Time.time;//dit werkt blijkbaar niet want starttime is hierna 0
        }
    }
}
