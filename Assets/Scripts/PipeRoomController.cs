using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PipeRoomController : MonoBehaviour
{
    public bool isButtonPresseed;
    public bool thumbsUpGestureOn;

    [Header("PIPE REFERENCES")]
    public List<Transform> pipePlacementTransforms;
    public List<Transform> pipeObjects;
    public List<GameObject> pipeTriggers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject _pipeTrigger in pipeTriggers)
        {
            _pipeTrigger.SetActive(isButtonPresseed);
        }
    }

    private void ThumbsUpGestureOn()
    {
        thumbsUpGestureOn = true;
    }

    private void ThumbsUpGestureOff()
    {
        thumbsUpGestureOn = false;
    }

    public void AttachPipe(int _pipeIndex)
    {
        Debug.LogFormat("<color=olive>Attach pipe {0}</color>", pipeObjects[_pipeIndex].name);
        //Remove XR Interactor
        pipeObjects[_pipeIndex].GetComponent<XRGrabInteractable>().enabled = false;

        pipeObjects[_pipeIndex].position = pipePlacementTransforms[_pipeIndex].position;
        pipeObjects[_pipeIndex].rotation = pipePlacementTransforms[_pipeIndex].rotation;
    }
}
