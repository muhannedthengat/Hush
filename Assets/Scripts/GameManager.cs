using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private PhotonView photonView;

    public List<Transform> playerSpawnPoints;

    [Header("SCRIPT REFERENCES")]
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public NetworkManager networkManager;

    [Header("CONTROLLER EVENTS")]
    public UnityEvent OnKillButtonPressed;
    public UnityEvent OnKillButtonReleased;

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

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButtonPressed()
    {
        SendStartGameEvent();
    }


    public void SendStartGameEvent()
    {
        if (playerController.player.playerId == 1)
        {
            //Select a random person to be a ghost
            int _randomIndex = Random.Range(1, networkManager.PlayerCount() + 1);
            //int _randomIndex = 1;

            Debug.LogFormat("<color=cyan>Player {0} will be the ghost </color>", _randomIndex);

            //Raise the Photon event for Game Start
            PhotonEventController.Instance.RaiseCustomEvent(StaticData.StartGameEventCode, new object[] { _randomIndex });
        }
        else
        {
            return;
        }
    }

    public void OnStartGameEvent(object[] _content)
    {
        int _indexForGhost = (int)_content[0];

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

        //Hide victim's network player
        _killedPlayerTransform.parent.gameObject.SetActive(false);
    }


}
