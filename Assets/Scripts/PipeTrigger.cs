using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeTrigger : MonoBehaviour
{
    public int pipeIndex;

    private PipeRoomController pipeRoomController;

    // Start is called before the first frame update
    void Start()
    {
        pipeRoomController = FindObjectOfType<PipeRoomController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogFormat("<color=green>Collider with {0}</color>", other.name);
        switch(pipeIndex)
        {
            case 0:
                if(other.name.Equals("Pipe_1"))
                    pipeRoomController.AttachPipe(pipeIndex);
                break;
            case 1:
                if (other.name.Equals("Pipe_2"))
                    pipeRoomController.AttachPipe(pipeIndex);
                break;
        }
    }
}
