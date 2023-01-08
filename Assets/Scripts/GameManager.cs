using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinMeetingRoom()
    {
        photonView.RPC(nameof(JoinMeetingRoomRPC), RpcTarget.AllBufferedViaServer, null);
    }

    [PunRPC]
    private void JoinMeetingRoomRPC()
    {
        //Load scene
        PhotonNetwork.LoadLevel(2);
    }
}
