using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData
{
    #region PHOTON EVENT CODES
    public static byte StartGameEventCode = 1;

    public static byte OnPlayerCollisionEnterEventCode = 2;
    public static byte OnPlayerCollisionExitEventCode = 3;
        
    public static byte PlayerKilledEventCode = 4;

    public static byte PipeRoomButtonPressedEventCode = 5;
    #endregion
}
