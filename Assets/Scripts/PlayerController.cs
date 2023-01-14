using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public HushPlayer player;
    public NetworkPlayer myNetworkPlayer;

    public GameObject youAreKillerIndicator;
    public GameObject youAreWorkerIndicator;

    public GameObject killPlayerIndicator;
    public GameObject youDiedIndicator;
    public TextMeshProUGUI victimNameText;


    public bool isCollidingWithOtherPlayer;
    public string otherPlayerName;
    public bool isPressingKillButton;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnKillButtonPressed.AddListener(OnKillButtonPressed);
        GameManager.Instance.OnKillButtonReleased.AddListener(OnKillButtonReleased);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.role == HushPlayerRoles.Ghost && player.state == HushPlayerStates.Hunting)
        {
            //Check if within killing distance with other players
            if(isCollidingWithOtherPlayer && otherPlayerName != "")
            {
                //Is in killing distance of other player

                //TODO: Add UI for showing the ghost that he/she can kill the player
                if(!killPlayerIndicator.activeSelf)
                {
                    killPlayerIndicator.SetActive(true);
                    victimNameText.text = otherPlayerName;
                }

                //TODO: IF PLAYER PRESSES A KILL BUTTON, THE OTHER PLAYER SHOULD DIE
                if(isPressingKillButton)
                {
                    //Raise event for player killed
                    PhotonEventController.Instance.RaiseCustomEvent(StaticData.PlayerKilledEventCode,
                        new object[] { player.name, otherPlayerName });

                    isCollidingWithOtherPlayer = false;
                    otherPlayerName = "";
                }
            }
            else
            {
                killPlayerIndicator.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Initialize Player 
    /// </summary>
    /// <param name="_playerId"></param>
    public void InitializePlayer(int _playerId)
    {
        player = new HushPlayer();
        player.playerId = _playerId;
        player.name = "Player " + _playerId;
        player.role = HushPlayerRoles.None;
        player.state = HushPlayerStates.Spawned;
    }

    public void AssignPlayerRoleAndState(int _ghostIndex)
    {
        if (player.playerId == _ghostIndex)
        {
            AssignPlayerRole(HushPlayerRoles.Ghost);
            AssignPlayerState(HushPlayerStates.Hunting);
        }
        else
        {
            AssignPlayerRole(HushPlayerRoles.Worker);
            AssignPlayerState(HushPlayerStates.Working);
        }
        StartCoroutine(DisplayRoleToPlayer(player.role));
    }

    public void AssignPlayerRole(HushPlayerRoles _role)
    {
        player.role = _role;
    }

    public void AssignPlayerState(HushPlayerStates _state)
    {
        player.state = _state;
    }

    public void KillPlayer(string _killerName)
    {
        AssignPlayerState(HushPlayerStates.Dead);

        //TODO: SHOW UI FOR DEATH
        youDiedIndicator.SetActive(true);
    }

    private void OnKillButtonPressed()
    {
        isPressingKillButton = true;
    }

    private void OnKillButtonReleased()
    {
        isPressingKillButton = false;
    }

    private IEnumerator DisplayRoleToPlayer(HushPlayerRoles _role)
    {
        switch(_role)
        {
            case HushPlayerRoles.Ghost:
                youAreKillerIndicator.SetActive(true);
                break;
            case HushPlayerRoles.Worker:
                youAreWorkerIndicator.SetActive(true);
                break;
            default:
                youAreWorkerIndicator.SetActive(true);
                break;
        }

        yield return new WaitForSeconds(3f);
        youAreKillerIndicator.SetActive(false);
        youAreWorkerIndicator.SetActive(false);
    }
}

[System.Serializable]
public class HushPlayer
{
    public int playerId;
    public string name;
    public HushPlayerRoles role;
    public HushPlayerStates state;
}

public enum HushPlayerRoles
{
    None,
    Worker,
    Ghost
}

public enum HushPlayerStates
{
    None,
    Spawned,
    Working,
    Hunting,
    Killing,
    Dead,
    Accusing,
    Voting
}
