using UnityEngine;
using System.Collections.Generic;
using Leap;
using UnityEngine.EventSystems;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
using TalesFromTheRift;

public class FingerPaint : MonoBehaviour
{
    public HandController LeapHandController;
    public SwypeController swypeController;
    public CanvasKeyboard keyboard;
    private bool swiping;
    private String curChar;
    private RaycastHit hit;
    public Camera camera;
    List<Vector3> linePoints;
    LineRenderer lineRenderer;
    FingerModel finger;
    Controller leapController = new Controller();
    private bool fingerdetect;
    private float newPointDelta = 0.02f;
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
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetVertexCount(0);
        lineRenderer.SetWidth(0.1f, 0.1f);
        lineRenderer.SetColors(Color.red, Color.red);
        lineRenderer.useWorldSpace = true;
        fingerdetect = false;
        linePoints = new List<Vector3>();
        curChar = " ";
        swiping = false;
    }

    // Update is called once per frame
    void Update()
    {

        var pointer = new PointerEventData(EventSystem.current);
       
        HandModel[] allGraphicHands = LeapHandController.GetAllGraphicsHands();

        if (allGraphicHands.Length <= 0)
            return;
        HandModel handModel = allGraphicHands[0];

        finger = handModel.fingers[(int)Finger.FingerType.TYPE_INDEX];

        fingerTipPos = finger.GetTipPosition();

        Frame frame = leapController.Frame();
        GestureList gestureInFrame = frame.Gestures();
        bool swipeDetected = false;

        foreach (Gesture g in gestureInFrame)
            if (!IsPointing(handModel) && g.Type == Gesture.GestureType.TYPE_SWIPE)
            {
                swipeDetected = true;
                if (!swiping)
                {
                    swiping = true;
                    SwipeGesture swiper = new SwipeGesture(g);
                    Vector3 dir = new Vector3(Mathf.Round(swiper.Direction.x), Mathf.Round(swiper.Direction.y), Mathf.Round(swiper.Direction.z));

                    Debug.Log("Swiping!: " + dir);
                    if (dir.y == -1 && keyboard.isActiveAndEnabled) // add swype speed
                        keyboard.CloseKeyboard();
                }
            }
        if (!swipeDetected)
            swiping = false;

        


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
                swypeController.EndOfInput();
                linePoints.Clear();

            }

            fingerdetect = false;
        }

        if (fingerdetect && keyboard.isActiveAndEnabled)
        {
            // Using named temp variables like this helps me think more clearly about the code
            Vector3 previousPoint = (linePoints.Count > 0) ? linePoints[linePoints.Count - 1] : new Vector3(-1000, -1000, -1000); // If you've never seen this before, it's called a ternary expression.
                                                                                                                                  // It's just an if/else collapsed into a single line of code. 
            
            // Also, the crazy out of bounds initial value here ensures the starting point will always draw.
            Vector3 dir = (fingerTipPos - camera.transform.position).normalized*30;

            Debug.DrawRay(fingerTipPos, dir, Color.red, 1, true);
            
            if (Physics.Raycast(fingerTipPos, dir, out hit, 1000F))
            {
               // Debug.Log("Collided with: " + hit.collider.gameObject.name);
                if (hit.collider.gameObject.name != curChar && hit.collider.gameObject.name.Length == 1)
                {
                    curChar = hit.collider.gameObject.name;
                    swypeController.AddCharacter(curChar[0]);
                }
            }
            
            if (Vector3.Distance(fingerTipPos, previousPoint) > newPointDelta)
            {
                linePoints.Add(fingerTipPos);
                lineRenderer.SetVertexCount(linePoints.Count);
                lineRenderer.SetPosition(linePoints.Count - 1, (Vector3)linePoints[linePoints.Count - 1]);
              //  Debug.Log(string.Format("Added point at: {0}!", fingerTipPos));
            }
        }
    }

    private bool IsPointing(HandModel handModel)
    {
        return handModel.fingers[(int)Finger.FingerType.TYPE_INDEX].GetLeapFinger().IsExtended && !handModel.fingers[(int)Finger.FingerType.TYPE_THUMB].GetLeapFinger().IsExtended && !handModel.fingers[(int)Finger.FingerType.TYPE_MIDDLE].GetLeapFinger().IsExtended && !handModel.fingers[(int)Finger.FingerType.TYPE_RING].GetLeapFinger().IsExtended && !handModel.fingers[(int)Finger.FingerType.TYPE_PINKY].GetLeapFinger().IsExtended;
    }


}