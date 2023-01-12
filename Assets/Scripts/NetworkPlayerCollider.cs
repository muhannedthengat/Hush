using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerCollider : MonoBehaviour
{
    public NetworkPlayer networkPlayer;

    private void Awake()
    {
        networkPlayer = GetComponentInParent<NetworkPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        NetworkPlayerCollider _npc = other.GetComponent<NetworkPlayerCollider>();
        if (_npc != null && _npc.networkPlayer != null)
        {
            //Debug.LogFormat("{0} Colliding with {1}", networkPlayer.gameObject.name, _npc.networkPlayer.gameObject.name);
            networkPlayer.OnCollisionEnterWithOtherPlayer(_npc.networkPlayer.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NetworkPlayerCollider _npc = other.GetComponent<NetworkPlayerCollider>();
        if (_npc != null && _npc.networkPlayer != null)
        {
            //Debug.LogFormat("{0} Not Colliding with {1}", networkPlayer.gameObject.name, _npc.networkPlayer.gameObject.name);
            networkPlayer.OnCollisionExitWithOtherPlayer(_npc.networkPlayer.gameObject.name);
        }
    }
}
