using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeetingRoomManager : MonoBehaviour
{
    [Header("TIMERS")]
    public float accusationPhaseTimeLimit;
    public float votingPhaseTimeLimit;

    [Header("UI REFERENCES")]
    public TextMeshProUGUI timerText;

    [Header("DEBUG ONLY")]
    [SerializeField] private float accusationPhaseTimer;
    [SerializeField] private float votingPhaseTimer;

    [SerializeField] private bool accusationPhaseOver;
    [SerializeField] private bool votingPhaseOver;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!accusationPhaseOver)
        {
            accusationPhaseTimer += Time.deltaTime;
            timerText.text = string.Format("Accuse {0}", (int)(accusationPhaseTimeLimit - accusationPhaseTimer));
            if (accusationPhaseTimer >= accusationPhaseTimeLimit)
            {
                accusationPhaseOver = true;
            }
        }
        else if(!votingPhaseOver)
        {
            votingPhaseTimer += Time.deltaTime;
            timerText.text = string.Format("Vote {0}", (int)(votingPhaseTimeLimit - votingPhaseTimer));
            if (votingPhaseTimer >= votingPhaseTimeLimit)
            {
                votingPhaseOver = true;
            }
        }
    }
}
