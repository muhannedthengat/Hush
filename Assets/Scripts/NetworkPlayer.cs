using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class NetworkPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    [Header("AVATAR REFERENCES")]
    [SerializeField] private List<Material> avatarMaterials;
    [SerializeField] private List<Renderer> avatarRenderers;

    private PhotonView photonView;
    private Transform headRig;
    private Transform rightHandRig;
    private Transform leftHandRig;

    public HushPlayer player;

    private GestureController gestureController;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>(); // to know if the prefab was spawned by us or another player
        FetchXRRig();

        if (photonView.IsMine)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
            GameManager.Instance.playerController.myNetworkPlayer = this;
        }

        //Assign name to network player
        gameObject.name = photonView.Owner.NickName;

        //Assign material to network player
        int playerIndex = GameManager.Instance.GetPlayerIndex(gameObject.name);
        Material _avatarMat = GameManager.Instance.avatarMaterials[playerIndex - 1];
        foreach(Renderer _rend in avatarRenderers)
        {
            _rend.material = _avatarMat;
        }

        gestureController = FindObjectOfType<GestureController>();
        gestureController.idleGestureOn.AddListener(SetHandAnimation);
        gestureController.fistGestureOn.AddListener(SetHandAnimation);
        gestureController.thumbsGestureOn.AddListener(SetHandAnimation);
        gestureController.pointGestureOn.AddListener(SetHandAnimation);
        gestureController.vSignGestureOn.AddListener(SetHandAnimation);
    }

    // Update is called once per frame
    void Update()
    {
        if(headRig == null)
        {
            FetchXRRig();
        }

        if(photonView.IsMine)
        {
            //rightHand.gameObject.SetActive(false); 
            //leftHand.gameObject.SetActive(false); 
            //head.gameObject.SetActive(false);

            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);

            //UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
            //UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
            
        }

    }

    private void FetchXRRig()
    {
        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
    }

    void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    private void SetHandAnimation(Gestures gesture)
    {
        switch (gesture)
        {
            case Gestures.None:
                rightHandAnimator.SetTrigger("Idle");
                break;
            case Gestures.Idle:
                rightHandAnimator.SetTrigger("Idle");
                break;
            case Gestures.Fist:
                rightHandAnimator.SetTrigger("Fist");
                break;
            case Gestures.Thumbs:
                rightHandAnimator.SetTrigger("Thumbs");
                break;
            case Gestures.Point:
                rightHandAnimator.SetTrigger("Point");
                break;
            case Gestures.VSign:
                rightHandAnimator.SetTrigger("VSign");
                break;
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation; 
    }

    public void OnCollisionEnterWithOtherPlayer(string _otherPlayerName)
    {
        if(photonView != null && photonView.IsMine)
        {
            //Raise event for collision enter
            PhotonEventController.Instance.RaiseCustomEvent(StaticData.OnPlayerCollisionEnterEventCode,
                new object[] { player.name, _otherPlayerName });
        }
    }

    public void OnCollisionExitWithOtherPlayer(string _otherPlayerName)
    {
        if (photonView != null && photonView.IsMine)
        {
            //Raise event for collision exit
            PhotonEventController.Instance.RaiseCustomEvent(StaticData.OnPlayerCollisionExitEventCode,
                new object[] { player.name, _otherPlayerName });
        }
    }
}
