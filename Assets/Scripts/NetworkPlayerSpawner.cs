using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        //spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
    }

    public void SpawnNetworkPlayer(HushPlayer _player)
    {
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
        spawnedPlayerPrefab.GetComponent<NetworkPlayer>().player = _player;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
