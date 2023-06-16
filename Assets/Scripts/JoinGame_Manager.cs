using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Unity.Multiplayer;
using Unity.Collections;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using System;

public class JoinGame_Manager : NetworkBehaviour
{
    //========
    //VARIABLES
    //========

    [Header("UI Ref")]
    public Button readyButton;
    public Button startButton;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private GameObject lobbyMenu_UI;
    [SerializeField] private TextMeshProUGUI streamerNameText;
    [SerializeField] private TextMeshProUGUI joinCodeText;
    [SerializeField] private List<SoldierLobby> soldierLobbies = new List<SoldierLobby>();

    [Header("Script Ref")]
    [SerializeField] private Testing_Relay relayScript;
    public List<GameObject> vivoxObject = new();

    [Header("Network Variable")]
    public NetworkVariable<bool> isTheGameStarted = new NetworkVariable<bool>(false);
    public NetworkVariable<FixedString32Bytes> joinCode = new NetworkVariable<FixedString32Bytes>("");

    //========
    //MONOBEHAVIOUR
    //========
    private void OnEnable()
    {
        startButton.onClick.AddListener(TryToStartGameServerRpc);
        relayScript.onCreateRoomSuccess.AddListener(SetJoinCode);
    }
    private void OnDisable()
    {
        startButton.onClick.RemoveListener(TryToStartGameServerRpc);
        relayScript.onCreateRoomSuccess.AddListener(SetJoinCode);
    }
    public override async void OnGainedOwnership()
    {
        /*await Task.Delay(1000);
        if(IsHost)
        {
            print("testing");
            joinCode.Value = relayScript.GetJoinCode();
        }*/
    }
    private void Update()
    {
        if(IsHost && !isTheGameStarted.Value)
        {
            UpdateSoldierLobbyClientRpc();
        }
    }

    //========
    //FONCTION
    //========
    public void OpenLobbyUI()
    {
        lobbyMenu_UI.SetActive(true);
        print("Opening Lobby");
    }
    public void QuitLobbyUI()
    {
        lobbyMenu_UI.SetActive(false);
        print("Removing Lobby");
    }
    public int GiveIdNotTaken()
    {
        PlayerConnectionManager[] playersManager = GameObject.FindObjectsOfType<PlayerConnectionManager>();
        List<int> playersIdLeft = new () { 0, 1, 2, 3, 4 };
        foreach(PlayerConnectionManager playerConnectionManager in playersManager)
        {
            Debug.Log("Removing" + playerConnectionManager.playerId.Value + "id");
            int idTaken = playerConnectionManager.playerId.Value;
            try
            {
                playersIdLeft.Remove(idTaken);
            }
            catch { Debug.LogError("List Error : " + idTaken); }
        }
        Debug.Log("Id not Taken = " + playersIdLeft[0]);
        return playersIdLeft[0];
    }
    [ClientRpc]
    public void UpdateSoldierLobbyClientRpc()
    {
        PlayerConnectionManager[] playersManager = GameObject.FindObjectsOfType<PlayerConnectionManager>();
        List<SoldierLobby> soldierNotInTheLobby = new();

        if(playersManager.Length - 1 > soldierLobbies.Count) Debug.LogError("Not Set UI in Reference");

        soldierNotInTheLobby.AddRange(soldierLobbies);

        foreach (PlayerConnectionManager playerConnectionManager in playersManager)
        {
            int idPlayer = playerConnectionManager.playerId.Value;
            if (idPlayer == 0) continue;
            SoldierLobby soldierLobby = soldierLobbies[idPlayer - 1];

            Debug.Log("Updating Player " + idPlayer + "UI");

            if(playerConnectionManager.isTheSoldierReady.Value)
            {
                soldierLobby.WhenSoliderIsReady();
            }
            else
            {
                soldierLobby.WhenSoliderJoin();
            }

            soldierNotInTheLobby.Remove(soldierLobby);
        }

        foreach(SoldierLobby soldier in soldierNotInTheLobby)
        {
            soldier.WhenWaitingForSoldier();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryToStartGameServerRpc()
    {
        PlayerConnectionManager[] playersManager = GameObject.FindObjectsOfType<PlayerConnectionManager>();
        bool canStartTheGame = true;
        if(!IsHost)
        {
            Debug.LogError("Only the host can start the game");
            return;
        }
        if (playersManager.Length != 5)
        {
            Debug.LogError("Not Enought Player");
            canStartTheGame = false;
            return;
        } 
        foreach(PlayerConnectionManager playerScript in playersManager)
        {
            if(!playerScript.isTheSoldierReady.Value && playerScript.playerId.Value != 0)
            {
                canStartTheGame = false;
                Debug.LogError("Player not ready");
                return;
            }
        }

        if(canStartTheGame)
        {
            StartTheGameClientRpc();
        }
    }

    [ClientRpc]
    public void StartTheGameClientRpc()
    {
        if(IsHost) isTheGameStarted.Value = true;
        int playerId = 5;
        PlayerConnectionManager[] playersManager = GameObject.FindObjectsOfType<PlayerConnectionManager>();
        foreach(PlayerConnectionManager playerScript in playersManager)
        {
            if(playerScript.IsOwner)
            {
                playerId = playerScript.playerId.Value;
            }
        }
        Debug.Log("Start the game, Incarnate Player " + playerId);
        In_Game_Manager.Instance.GiveInputAndCameraToPlayer(playerId);
        GiveOwnerToPlayerServerRpc(playerId, NetworkManager.Singleton.LocalClientId);
        QuitLobbyUI();
    }

    [ServerRpc(RequireOwnership = false)]
    public void GiveOwnerToPlayerServerRpc(int playerid, ulong clientId)
    {
        In_Game_Manager startGame_Manager = FindAnyObjectByType<In_Game_Manager>();

        GameObject player = startGame_Manager.GetPlayerViaId(playerid);

        NetworkObject[] networkObjects = player.GetComponentsInChildren<NetworkObject>();

        foreach (NetworkObject networkObject in networkObjects)
        {
            networkObject.GetComponent<NetworkObject>().ChangeOwnership(clientId);
            Debug.Log("Switch Ownership to Client " + clientId);
        }
    }
    private void SetJoinCode()
    {
        string currentJoinCode = relayScript.GetJoinCode();
        joinCodeText.text = currentJoinCode;
        joinCode.Value = currentJoinCode;
    }
}
