using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class DragHandler : MonoBehaviour, IDragHandler, IPointerEnterHandler {


    // Use this for initialization
    void Start() {

    }
    // Update is called once per frame
    void Update () {

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Entering");
    }
}
