using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using NaughtyAttributes;

public class Screenshake_Manager : NetworkBehaviour
{
    //========
    //VARIABLES
    //========
    [SerializeField] private CinemachineImpulseSource impulseSourceL;
    [SerializeField] private CinemachineImpulseSource impulseSourceM;
    [SerializeField] private CinemachineImpulseSource impulseSourceS;
    [SerializeField] private CinemachineImpulseSource impulseSourceExtraS;

    //========
    //FONCTION
    //========
    [ClientRpc]
    public void ScreenshakeClientRpc(Screenshake type)
    {
        ScreenshakeSolo(type);
    }
    public void ScreenshakeSolo(Screenshake type)
    {
        switch (type)
        {
            case Screenshake.Extra_S:
                impulseSourceExtraS.GenerateImpulse();
                break;
            case Screenshake.Small:
                impulseSourceS.GenerateImpulse();
                break;
            case Screenshake.M:
                impulseSourceM.GenerateImpulse();
                break;
            case Screenshake.Large:
                impulseSourceL.GenerateImpulse();
                break;
        }
    }
}
public enum Screenshake
{
    Extra_S, Small, M, Large
}
