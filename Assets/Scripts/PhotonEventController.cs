using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class PhotonEventController : MonoBehaviour
{
    private static PhotonEventController _instance;
    public static PhotonEventController Instance { get { return _instance; } }

    private GameManager gameManager;

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
        gameManager = GameManager.Instance;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnCustomEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnCustomEvent;
    }

    /// <summary>
    /// Raises a custom Photon event using the given parameters
    /// </summary>
    public void RaiseCustomEvent(byte _eventCode, object[] _content)
    {
        Debug.LogFormat("<color=green>RaiseCustomEvent {0} | {1}</color>", _eventCode, _content);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache };
        PhotonNetwork.RaiseEvent(_eventCode, _content, raiseEventOptions, SendOptions.SendReliable);
    }

    /// <summary>
    /// This function receives the custom RPC events sent by any player
    /// </summary>
    /// <param name="photonEvent"></param>
    public void OnCustomEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == StaticData.StartGameEventCode)
            gameManager.OnStartGameEvent((object[])photonEvent.CustomData);
        else if (eventCode == StaticData.OnPlayerCollisionEnterEventCode)
            gameManager.OnCollisionEnterWithOtherPlayer((object[])photonEvent.CustomData);
        else if (eventCode == StaticData.OnPlayerCollisionExitEventCode)
            gameManager.OnCollisionExitWithOtherPlayer((object[])photonEvent.CustomData);
        else if (eventCode == StaticData.PlayerKilledEventCode)
            gameManager.OnPlayerKilled((object[])photonEvent.CustomData);
        else if (eventCode == StaticData.PipeRoomButtonPressedEventCode)
        {
            object[] content = (object[])photonEvent.CustomData;
            if (FindObjectOfType<ButtonTrigger>() != null)
            {
                bool _isButtonPresseed = (bool)content[0];
                FindObjectOfType<ButtonTrigger>().OnButtonPressEvent(_isButtonPresseed);
            }
        }
        else if (eventCode == StaticData.EmergencyMeetingEventCode)
            gameManager.OnEmergencyMeetingEvent((object[])photonEvent.CustomData);
    }
}
