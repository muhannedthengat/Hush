using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public bool showController = false;
    public bool showGestureAnimation;

    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    public GameObject handModelPrefab;
    
    private InputDevice targetDevice;
    private GameObject spawnedController;
    private GameObject spawnedHandModel;
    private Animator handAnimator;

    private GestureController gestureController;

    // Start is called before the first frame update
    void Start()
    {
        TryInitialize();

        if(showGestureAnimation)
        {
            gestureController = FindObjectOfType<GestureController>();
            gestureController.idleGestureOn.AddListener(SetHandAnimation);
            gestureController.fistGestureOn.AddListener(SetHandAnimation);
            gestureController.thumbsGestureOn.AddListener(SetHandAnimation);
            gestureController.pointGestureOn.AddListener(SetHandAnimation);
            gestureController.vSignGestureOn.AddListener(SetHandAnimation);
        }
        
    }

    private void OnDisable()
    {
        gestureController = FindObjectOfType<GestureController>();
        if(gestureController != null)
        {
            gestureController.idleGestureOn.RemoveListener(SetHandAnimation);
            gestureController.fistGestureOn.RemoveListener(SetHandAnimation);
            gestureController.thumbsGestureOn.RemoveListener(SetHandAnimation);
            gestureController.pointGestureOn.RemoveListener(SetHandAnimation);
            gestureController.vSignGestureOn.RemoveListener(SetHandAnimation);
        }
    }

    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            if (prefab)
            {
                spawnedController = Instantiate(prefab, transform);
            }
            else
            {
                Debug.Log("Did not find corresponding controller model");
            }

            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponentInChildren<Animator>();
        }
    }

    void UpdateHandAnimation()
    {
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            //handAnimator.SetFloat("Trigger", triggerValue);
            handAnimator.SetFloat("Punch", triggerValue);
        }
        else
        {
            //handAnimator.SetFloat("Trigger", 0);
            handAnimator.SetFloat("Punch", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            //Debug.LogFormat("Grip Value {0}", gripValue);
            handAnimator.SetFloat("Thumbs up", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Thumbs up", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryValue))
        {
            //Debug.LogFormat("Primary value {0}", primaryValue);
            handAnimator.SetBool("Thumbs down", primaryValue);
        }
        //else
        //{
        //    handAnimator.SetBool("Thumbs down", primaryValue);
        //}
        if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValue))
        {
            handAnimator.SetBool("V sign", secondaryValue);
        }
    }

    private void SetHandAnimation(Gestures gesture)
    {
        switch(gesture)
        {
            case Gestures.None:
                handAnimator.SetTrigger("Idle");
                break;
            case Gestures.Idle:
                handAnimator.SetTrigger("Idle");
                break;
            case Gestures.Fist:
                handAnimator.SetTrigger("Fist");
                break;
            case Gestures.Thumbs:
                handAnimator.SetTrigger("Thumbs");
                break;
            case Gestures.Point:
                handAnimator.SetTrigger("Point");
                break;
            case Gestures.VSign:
                handAnimator.SetTrigger("VSign");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!targetDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            if (showController)
            {
                if(spawnedHandModel)
                    spawnedHandModel.SetActive(false);
                if(spawnedController)
                    spawnedController.SetActive(true);
            }
            else
            {
                if (spawnedHandModel)
                    spawnedHandModel.SetActive(true);
                if (spawnedController)
                    spawnedController.SetActive(false);
                //UpdateHandAnimation();
            }
        }
    }
}
