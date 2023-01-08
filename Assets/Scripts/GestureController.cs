using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public enum Gestures
{
    None,
    Idle,
    Fist,
    Thumbs,
    Point,
    VSign
}

public class GestureController : MonoBehaviour
{
    private static GestureController _instance;
    public static GestureController Instance { get { return _instance; } }

    public UnityEvent<Gestures> idleGestureOn;
    public UnityEvent<Gestures> idleGestureOff;

    public UnityEvent<Gestures> fistGestureOn;
    public UnityEvent<Gestures> fistGestureOff;

    public UnityEvent<Gestures> thumbsGestureOn;
    public UnityEvent<Gestures> thumbsGestureOff;

    public UnityEvent<Gestures> pointGestureOn;
    public UnityEvent<Gestures> pointGestureOff;

    public UnityEvent<Gestures> vSignGestureOn;
    public UnityEvent<Gestures> vSignGestureOff;

    private bool lastBButtonState;
    private int bButtonPressCount = 1;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand) != null)
        {
            //Toggle Game Menu based on user input
            bool isYPressed, tempState = false;
            tempState = InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.secondaryButton, out isYPressed)
                && isYPressed || tempState;

            if (tempState != lastBButtonState) // Button state changed since last frame
            {
                Debug.LogFormat("<color=white>B IS PRESSED {0} | {1}</color>", bButtonPressCount,(bButtonPressCount % 2));
                lastBButtonState = tempState;

                if(bButtonPressCount % 2 == 0)
                {
                    EmoteWheelController ewController = FindObjectOfType<EmoteWheelController>(true);
                    Debug.LogFormat("<color=white>ewController</color>", ewController);


                    //Toggle Emote Wheel
                    if (ewController != null)
                    {
                        ewController.gameObject.SetActive(!ewController.gameObject.activeSelf);
                    }
                }
                bButtonPressCount++;

            }
        }
    }
}
