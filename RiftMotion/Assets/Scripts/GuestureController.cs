using UnityEngine;
using System.Collections.Generic;
using Leap;
using UnityEngine.EventSystems;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
using TalesFromTheRift;
using System.Linq;

public class GuestureController : MonoBehaviour
{
    public HandController LeapHandController;
    public SwypeController swypeController;
    public CanvasKeyboard keyboard;
    private bool swiping;
    private String curChar;
    private RaycastHit hit;
    public Painter painter;
    public Camera camera;
    public HandModel handModel;
    //List<Vector3> linePoints;
    //LineRenderer lineRenderer;
    FingerModel finger;
    Controller leapController = new Controller();
    private bool fingerdetect;
    private float duration;
    
    private Vector3 fingerTipPos;
    private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
    [DllImport("user32.dll")]
    private static extern void mouse_event(
       UInt32 dwFlags, // motion and click options
       UInt32 dx, // horizontal position or change
       UInt32 dy, // vertical position or change
       UInt32 dwData, // wheel movement
       IntPtr dwExtraInfo); // application-defined information


    // Use this for initialization
void Awake()
    {
        
        //SampleListener listenerSubclass = new SampleListener();
        leapController.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
        leapController.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
        leapController.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        /*
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetVertexCount(0);
        lineRenderer.SetWidth(0.1f, 0.1f);
        lineRenderer.SetColors(Color.red, Color.red);
        lineRenderer.useWorldSpace = true;
        linePoints = new List<Vector3>();
        */
        fingerdetect = false;
        curChar = " ";
        swiping = false;
    }

    // Update is called once per frame
    void Update()
    {       
         
        HandModel[] allGraphicHands = LeapHandController.GetAllGraphicsHands();

        if (allGraphicHands.Length <= 0) //no hands present
            return;

        HandModel handModel = allGraphicHands[0]; //only 1 hand? Might overthink this (I've got no better idea atm)
        this.handModel = handModel;

        GestureUpdate(handModel); //handels gestures (eg for closing keyboard
        PointingUpdate(handModel); //checks if pointing, sends raycasts and draws line
    }

    private void SwipeGesture(Gesture g, HandModel hand)
    {
        bool swipeDetected = false;

        if (!IsPointing(hand) && g.Type == Gesture.GestureType.TYPE_SWIPE)
        {
            swipeDetected = true;
            if (!swiping)
            {
                swiping = true;
                SwipeGesture swiper = new SwipeGesture(g);
                Vector3 dir = new Vector3(Mathf.Round(swiper.Direction.x), Mathf.Round(swiper.Direction.y), Mathf.Round(swiper.Direction.z));

                if (dir.y == -1 && keyboard.isActiveAndEnabled) // add swype speed
                    keyboard.CloseKeyboard();
                if (dir.y == 1 && !keyboard.isActiveAndEnabled) // add swype speed
                    keyboard.OpenKeyboard();
            }
        }
        if (!swipeDetected)
            swiping = false;
    }

    private void SelectGesture(Gesture g, HandModel hand)
    {
        if (g.Type == Gesture.GestureType.TYPE_SCREEN_TAP || g.Type == Gesture.GestureType.TYPE_KEY_TAP) //Actually never sees the screen tap, need to check this out
        {

            GameObject tapped = fireRaycasts(hand.fingers[(int)Finger.FingerType.TYPE_INDEX].GetTipPosition());
            if (tapped != null)
                checkObjectTap(tapped);

        }
    }

    private void GestureUpdate(HandModel hand)
    {
        Frame frame = leapController.Frame();
        GestureList gestureInFrame = frame.Gestures();
        

        foreach (Gesture g in gestureInFrame)
        {
            SwipeGesture(g, hand); //checks if it is a swipe gesture and takes action if needed
            SelectGesture(g, hand);
        }
           
    }

    private void PointingUpdate(HandModel handModel)
    {
        if (IsPointing(handModel))
        {
            //ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);Hoe we een event willen executen
            fingerdetect = true;
        }
        else
        {
            //ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
            if (fingerdetect)
            {
                if(keyboard.isActiveAndEnabled) //Otherwise causes nullpointer exeptions
                    swypeController.EndOfInput();
                painter.removeLine();
            }

            fingerdetect = false;
        }

        if (fingerdetect && keyboard.isActiveAndEnabled)
        {
            fingerTipPos = handModel.fingers[(int)Finger.FingerType.TYPE_INDEX].GetTipPosition();
            GameObject hit = fireRaycasts(fingerTipPos);
            if (hit != null)
                checkObjectDrag(hit);
            
        }
    }

    private void checkObjectTap(GameObject tapped)
    {
        if (tapped.CompareTag("TextOutput") && !keyboard.isActiveAndEnabled)
            keyboard.OpenKeyboard();


        if (tapped.CompareTag("SuggestionField"))
        {
            swypeController.setSuggestion(int.Parse(tapped.name[tapped.name.Length - 1].ToString()));
        }
        else if (tapped.CompareTag("Key") && tapped.name != curChar)
        {
            curChar = tapped.name;
            swypeController.Typing(curChar[0]);
        }
        else if(tapped.CompareTag("Space"))
        {
            swypeController.Typing(' ');
        }
        

    }

    private void checkObjectDrag(GameObject hit)
    {
        if (hit.CompareTag("Key") && hit.name != curChar)
        {
            if (duration != 0) //don't sent first time
                swypeController.AddCharacter(curChar[0], duration);//send previous character

            curChar = hit.name;
            duration = 0;
            //swypeController.AddCharacter(curChar[0]);
        }
        if (hit.CompareTag("Key") && hit.name == curChar)
            duration += Time.deltaTime;       
        if(partOfKeyBoard(hit))
            painter.addPoint(fingerTipPos);

    }

    private bool partOfKeyBoard(GameObject entity)
    {

        if (entity.CompareTag("Keyboard"))
            return true;
        else if (entity.transform.parent != null)
            return partOfKeyBoard(entity.transform.parent.gameObject);
        else 
            return false;
    }


    private GameObject fireRaycasts(Vector3 pos)
    {
        Vector3 tip=handModel.fingers[1].GetTipPosition();
        Vector3 dir = tip-Camera.main.transform.position;//(camera.transform.forward);
        Debug.DrawRay(pos, dir*5f, Color.red, 1, true);
       
        if (Physics.Raycast(pos, dir, out hit, 1000F))
        {
            // Debug.Log("Collided with: " + hit.collider.gameObject.name);
            return hit.collider.gameObject;
        }
        else
            return null;
    }
    /// <summary>
    /// Returns list of booleans for each finger that is extending.
    /// Indices
    /// Thumbs = 0
    /// Indexfinger = 1
    /// Middelfinger = 2
    /// Ringfinger = 3
    /// Littlefinger = 4
    /// </summary>
    /// <returns></returns>
    private bool[] ExtendingFingers(HandModel hand)
    {
        bool[] extending = new bool[5];
        for(int i = 0; i < extending.Length; i++)
            extending[i] = hand.fingers[i].GetLeapFinger().IsExtended;

        return extending;

    }

    private bool isFist(HandModel hand)
    {
        return ExtendingFingers(hand).Count(c => c) == 0; //no fingers are extended
    }

    private bool isOpenHand(HandModel hand)
    {
        return ExtendingFingers(hand).Count(c => c) == 5; //all fingers are stretched
    }



    private bool IsPointing(HandModel hand)
    {
        bool[] extending = ExtendingFingers(hand);
        return extending[1] && extending.Count(c => c) == 1; //the indexfinger is extended and it is the only finger that is extended
        
    }


}