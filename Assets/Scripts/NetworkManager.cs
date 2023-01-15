using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class DefaultRoom
{
    public string Name;
    public int sceneIndex;
    public int maxPlayers;
}
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public List<DefaultRoom> defaultRooms;
    public GameObject roomUI;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ConnectToServer();
    }

    // Start is called before the first frame update
    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("-Connecting to Server-");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server.");
        base.OnConnectedToMaster();
        //PhotonNetwork.JoinLobby();
        InitializeRoom(0);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined the lobby.");
        roomUI.SetActive(true);
    }

    public void InitializeRoom(int defaultRoomIndex)
    {
        DefaultRoom roomSettings = defaultRooms[defaultRoomIndex];
        //Load scene
        //PhotonNetwork.LoadLevel(roomSettings.sceneIndex);

        //Creates a room
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) roomSettings.maxPlayers;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true; 
        PhotonNetwork.JoinOrCreateRoom(roomSettings.Name, roomOptions, TypedLobby.Default);
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room.");
        base.OnJoinedRoom();

        int playerCount = PlayerCount();
        PhotonNetwork.LocalPlayer.NickName = "Player " + playerCount;
        Debug.LogFormat("<color=cyan>Joined a room with {0} players</color>", playerCount);

        //Initialize local player
        GameManager.Instance.playerController.InitializePlayer(playerCount);

        //Initialize network player
        FindObjectOfType<NetworkPlayerSpawner>().SpawnNetworkPlayer(GameManager.Instance.playerController.player);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player has joined the room.");
        base.OnPlayerEnteredRoom(newPlayer);

        int playerCount = PlayerCount();
        Debug.LogFormat("<color=cyan>Player {0} entered the room with {1} players</color>",
            newPlayer.NickName, playerCount);

    }

    //TEMPORARY CODE. PLEASE DELETE THIS!!!!!!
    private void TemporaryCodeStart()
    {
        Debug.LogFormat("<color=red> *** STARTING THE GAME FOR DEBUG PURPOSE ***</color>");
        Debug.LogFormat("<color=red> *** DELETE THIS CODE LATER ***</color>");
        GameManager.Instance.StartButtonPressed();
    }

    /// <summary>
    /// Returns the number of players in the current room
    /// </summary>
    /// <returns></returns>
    public int PlayerCount()
    {
        return PhotonNetwork.PlayerList.Length;
    }
}
