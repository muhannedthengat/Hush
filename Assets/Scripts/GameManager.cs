using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

    [Header("SCRIPT REFERENCES")]
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public NetworkManager networkManager;

    [Header("CONTROLLER EVENTS")]
    public UnityEvent OnKillButtonPressed;
    public UnityEvent OnKillButtonReleased;

    public bool gameStarted;

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
        startGamePanel.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButtonPressed()
    {
        if(!gameStarted)
            SendStartGameEvent();
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

            startGamePanel.SetActive(false);
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
        startGamePanel.SetActive(false);

        //Assign local player's role as per the received index
        playerController.AssignPlayerRoleAndState(_indexForGhost);
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


}
