using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using GameplayIngredients;
using System;

public class PlayerConnectionManager : NetworkBehaviour
{
    //========
    //VARIABLES
    //========

    [SerializeField] private Callable onDisconnect;
    public NetworkVariable<int> playerId { private set; get; } = new NetworkVariable<int>(0);
    [SerializeField] private SoldierLobby soldierLobby;
    public NetworkVariable<bool> isTheSoldierReady { private set; get; } = new NetworkVariable<bool>(false);
    //========
    //MONOBEHAVIOUR
    //========

    public override void OnNetworkSpawn()
    {
        Debug.Log("New Player Connect: " +
        "IsOwner = " + IsOwner.ToString() +
        "IsServer = " + IsServer.ToString() +
        "IsClient = " + IsClient.ToString());


        JoinGame_Manager joinGame = FindAnyObjectByType<JoinGame_Manager>();

        if (IsOwner) 
        { 
            joinGame.OpenLobbyUI();
            if(!IsHost)
            joinGame.readyButton.onClick.AddListener(SoldierReadyButtonFlipFlopServerRpc);
        }

        if (IsHost)
        {
            if (IsOwnedByServer) playerId.Value = 0;
            else playerId.Value = joinGame.GiveIdNotTaken();
            Debug.Log("PlayerId = " + playerId.Value);

            joinGame.UpdateSoldierLobbyClientRpc();
        }
        if (IsOwner) ConnectToVivox();
    }

    private new void OnDestroy()
    {
        JoinGame_Manager joinGame = FindAnyObjectByType<JoinGame_Manager>();
        if (IsOwner)
        {
            joinGame.QuitLobbyUI();
            joinGame.readyButton.onClick.RemoveListener(SoldierReadyButtonFlipFlopServerRpc);
        }
    }
    //========
    //FONCTION
    //========

    [ServerRpc]
    public void SoldierReadyButtonFlipFlopServerRpc()
    {
        isTheSoldierReady.Value = !isTheSoldierReady.Value;
        if (isTheSoldierReady.Value) soldierLobby.WhenSoliderIsReady();

        JoinGame_Manager joinGame = FindAnyObjectByType<JoinGame_Manager>();
        joinGame.UpdateSoldierLobbyClientRpc();
    }
    public void SetVivoxOn()
    {
        if (playerId.Value != 0)
        {
            JoinGame_Manager joinGame = FindAnyObjectByType<JoinGame_Manager>();
            joinGame.vivoxObject[playerId.Value - 1].SetActive(true);
        }
    }
    private async void ConnectToVivox()
    {
        await System.Threading.Tasks.Task.Delay(50);

        LoginCredentials.Instance.SetUserName(playerId.Value.ToString());
        LoginCredentials.Instance.Login();
    }

}
