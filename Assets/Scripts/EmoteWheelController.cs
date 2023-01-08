using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteWheelController : MonoBehaviour
{
    private GestureController gestureController;
    // Start is called before the first frame update
    void Start()
    {
        gestureController = FindObjectOfType<GestureController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEmoteSelection(string _gesture)
    {
        Debug.LogFormat("<color=green> OnEmoteSelection {0}</color>", _gesture);
        switch(_gesture)
        {
            case "None":
                gestureController.idleGestureOn?.Invoke(Gestures.None);
                break;
            case "Idle":
                gestureController.idleGestureOn?.Invoke(Gestures.Idle);
                break;
            case "Fist":
                gestureController.fistGestureOn?.Invoke(Gestures.Fist);
                break;
            case "Thumbs":
                gestureController.thumbsGestureOn?.Invoke(Gestures.Thumbs);
                break;
            case "Point":
                gestureController.pointGestureOn?.Invoke(Gestures.Point);
                break;
            case "VSign":
                gestureController.vSignGestureOn?.Invoke(Gestures.VSign);
                break;
        }
        this.gameObject.SetActive(false);
    }
}
