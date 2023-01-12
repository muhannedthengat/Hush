using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public Material buttonOnMat;
    public Material buttonOffMat;

    private MeshRenderer buttonMesh;
    private PipeRoomController pipeRoomController;

    // Start is called before the first frame update
    void Start()
    {
        buttonMesh = GetComponent<MeshRenderer>();
        buttonMesh.material = buttonOffMat;

        pipeRoomController = FindObjectOfType<PipeRoomController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //buttonMesh.material = buttonOnMat;
        //pipeRoomController.isButtonPresseed = true;

        PhotonEventController.Instance.RaiseCustomEvent(StaticData.PipeRoomButtonPressedEventCode,
            new object[] { true });

    }

    private void OnTriggerExit(Collider other)
    {
        //buttonMesh.material = buttonOffMat;
        //pipeRoomController.isButtonPresseed = false;

        PhotonEventController.Instance.RaiseCustomEvent(StaticData.PipeRoomButtonPressedEventCode,
            new object[] { false });
    }

    public void OnButtonPressEvent(bool state)
    {
        buttonMesh.material = state ? buttonOnMat : buttonOffMat;
        pipeRoomController.isButtonPresseed = state;
    }
}
