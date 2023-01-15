using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private PhotonView photonView;

    [Header("PLAYER ESSENTIALS")]
    public List<Transform> playerSpawnPoints;
    public List<Material> avatarMaterials;

    [Header("MEETING ROOM UI")]
    public GameObject startGamePanel;
    public GameObject completeTaskPanel;
    public GameObject accusationPhasePanel;
    public GameObject votingPhasePanel;
    public GameObject votingResultPanel;

    [Header("TIMER TEXTS")]
    public TextMeshProUGUI accusationPhaseTimerText;
    public TextMeshProUGUI votingPhaseTimerText;

    [Header("TIMER LIMITS")]
    public float accusationPhaseTimeLimit;
    public float votingPhaseTimeLimit;

    [Header("DEBUG ONLY")]
    [SerializeField] private float accusationPhaseTimer;
    [SerializeField] private float votingPhaseTimer;

    [Header("SCRIPT REFERENCES")]
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public NetworkManager networkManager;

    [Header("CONTROLLER EVENTS")]
    public UnityEvent OnKillButtonPressed;
    public UnityEvent OnKillButtonReleased;

    [Header("GAME BOOLS")]
    public bool gameStarted;
    public bool emergencyMeetingCalled;
    public bool accusationPhaseOver;
    public bool votingPhaseOver;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        playerController = FindObjectOfType<PlayerController>();
        networkManager = FindObjectOfType<NetworkManager>();

        gameStarted = false;
        emergencyMeetingCalled = false;
        ShowUIPanel(0);

    }

    // Update is called once per frame
    void Update()
    {
        if(emergencyMeetingCalled)
        {
            if (!accusationPhaseOver)
            {
                accusationPhaseTimer += Time.deltaTime;
                accusationPhaseTimerText.text = string.Format("{0}", (int)(accusationPhaseTimeLimit - accusationPhaseTimer));
                if (accusationPhaseTimer >= accusationPhaseTimeLimit)
                {
                    accusationPhaseOver = true;

                    //Start voting phase
                    ShowUIPanel(3);
                }
            }
            else if (!votingPhaseOver)
            {
                votingPhaseTimer += Time.deltaTime;
                votingPhaseTimerText.text = string.Format("{0}", (int)(votingPhaseTimeLimit - votingPhaseTimer));
                if (votingPhaseTimer >= votingPhaseTimeLimit)
                {
                    votingPhaseOver = true;
                    emergencyMeetingCalled = false;

                    //TODO: SHOW VOTING RESULTS
                    ShowUIPanel(4);

                }
            }
        }
    }

    public void StartButtonPressed()
    {
        if(!gameStarted)
            SendStartGameEvent();
    }

    public void EmergencyMeetingButtonPressed()
    {
        if(gameStarted)
        {
            //Raise the Photon event for Emergency Meeting
            PhotonEventController.Instance.RaiseCustomEvent(StaticData.EmergencyMeetingEventCode, new object[] { playerController.player.playerId });

            ShowUIPanel(-1);
        }
    }


    public void SendStartGameEvent()
    {
        if (playerController.player.playerId == 1)
        {
            //Select a random person to be a ghost
            int _randomIndex = UnityEngine.Random.Range(1, networkManager.PlayerCount() + 1);
            //int _randomIndex = 1;

            Debug.LogFormat("<color=cyan>Player {0} will be the ghost </color>", _randomIndex);

            //Raise the Photon event for Game Start
            PhotonEventController.Instance.RaiseCustomEvent(StaticData.StartGameEventCode, new object[] { _randomIndex });

            ShowUIPanel(-1);
        }
        else
        {
            return;
        }
    }


    public void OnStartGameEvent(object[] _content)
    {
        int _indexForGhost = (int)_content[0];

        gameStarted = true;
        StartCoroutine(ShowCompleteTaskPanel());

        //Assign local player's role as per the received index
        playerController.AssignPlayerRoleAndState(_indexForGhost);
    }

    public void OnEmergencyMeetingEvent(object[] _content)
    {
        int _meetingInitiatorPlayerIndex = (int)_content[0];

        //Place player in meeting room
        playerController.PlacePlayerInMeetingRoom();

        //Show accusation phase UI
        ShowUIPanel(2);

        //Start accusation phase timer
        emergencyMeetingCalled = true;
        accusationPhaseOver = false;
        votingPhaseOver= false;
        accusationPhaseTimer = 0;
        votingPhaseTimer = 0;
    }

    private IEnumerator ShowCompleteTaskPanel()
    {
        yield return new WaitForSeconds(5f);
        ShowUIPanel(1);
    }

    public void OnCollisionEnterWithOtherPlayer(object[] _content)
    {
        string _localPlayerName = _content[0].ToString();
        string _otherPlayerName = _content[1].ToString();

        Debug.LogFormat("<color=magenta> {0} Colliding with {1}</color>",
            _localPlayerName, _otherPlayerName);

        if (playerController.player.name.Equals(_localPlayerName))
        {
            //This player is in collision area of other player
            playerController.isCollidingWithOtherPlayer= true;
            playerController.otherPlayerName= _otherPlayerName;
        }
    }

    public void OnCollisionExitWithOtherPlayer(object[] _content)
    {
        string _localPlayerName = _content[0].ToString();
        string _otherPlayerName = _content[1].ToString();

        Debug.LogFormat("<color=orange> {0} NOT Colliding with {1}</color>",
            _localPlayerName, _otherPlayerName);

        if (playerController.player.name.Equals(_localPlayerName))
        {
            //This player is out of collision area of other player
            playerController.isCollidingWithOtherPlayer = false;
            playerController.otherPlayerName = "";
        }
    }

    public void OnPlayerKilled(object[] _content)
    {
        string _killerName = _content[0].ToString();
        string _victimName = _content[1].ToString();

        //If local player is victim, change it's role to dead
        if(playerController.player.name.Equals(_victimName))
        {
            playerController.KillPlayer(_killerName);
        }

        //TODO: SPAWN A DEAD BODY AT THE PLAYER'S LOCATION
        Transform _killedPlayerTransform = GameObject.Find(_victimName).transform.Find("Head");
        Vector3 _spawnPos = new Vector3(_killedPlayerTransform.position.x, 0, _killedPlayerTransform.position.z);
        GameObject _deadBody = PhotonNetwork.Instantiate("Dead Body", _spawnPos, Quaternion.identity);
        _deadBody.name = "Dead Body of " + _victimName;

        //Assign material to dead body
        int _victimIndex = GetPlayerIndex(_victimName);
        _deadBody.GetComponent<DeadBody>().AssignAvatarMat(avatarMaterials[_victimIndex - 1]);

        //Hide victim's network player
        _killedPlayerTransform.parent.gameObject.SetActive(false);
    }

    public int GetPlayerIndex(string _playerName)
    {
        int _playerIndex = Int32.Parse(Regex.Match(_playerName, @"\d+").Value);
        return _playerIndex;
    }

    /// <summary>
    /// 0 = Start button | 1 = Complete Task | 
    /// 2 = Accusation Phase | 3 = Voting Phase |
    /// 4 = Voting Result
    /// </summary>
    /// <param name="index"></param>
    public void ShowUIPanel(int _index)
    {
        startGamePanel.SetActive(false);
        completeTaskPanel.SetActive(false);
        accusationPhasePanel.SetActive(false);
        votingPhasePanel.SetActive(false);
        votingResultPanel.SetActive(false);

        switch (_index)
        {
            case 0:
                startGamePanel.SetActive(true);
                break;
            case 1:
                completeTaskPanel.SetActive(true);
                break;
            case 2:
                accusationPhasePanel.SetActive(true);
                break;
            case 3:
                votingPhasePanel.SetActive(true);
                break;
            case 4:
                votingResultPanel.SetActive(true);
                break;
            default: 
                break;
        }
    }


}
