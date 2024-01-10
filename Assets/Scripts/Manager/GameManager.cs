using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<bool> OnLocalPlayerReadyChanged;

    private Dictionary<ulong, bool> playerReadyDictionary;
    [SerializeField] private PlayMode playMode;
    private bool isLocalPlayerReady = false;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();

        if (PlayerPrefs.GetInt("PlayMode") == 1)
        {
            playMode = PlayMode.MultiplayerLocal;
        }
        else if (PlayerPrefs.GetInt("PlayMode") == 2)
        {
            playMode = PlayMode.MultiplayerCom;
        }
        else if (PlayerPrefs.GetInt("PlayMode") == 3)
        {
            playMode = PlayMode.MultiplayerOnline;
        }
    }

    private void Start()
    {
        PlayersInputHandler.Instance.OnPlayerReady += PlayerInputHandler_OnPlayerReady;
    }
    private void OnDisable()
    {
        PlayersInputHandler.Instance.OnPlayerReady -= PlayerInputHandler_OnPlayerReady;
    }

    private void PlayerInputHandler_OnPlayerReady(object sender, EventArgs e)
    {
        isLocalPlayerReady = true;
        OnLocalPlayerReadyChanged?.Invoke(isLocalPlayerReady);

        //Debug.Log("Clients Total " + NetworkManager.Singleton.ConnectedClients.Count);
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        playerReadyDictionary[senderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                // Player is not ready or not found in the dictionary
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady && NetworkManager.Singleton.ConnectedClientsIds.Count == 2)
        {
            Debug.Log("Start Match");
        }
        //SetPlayerReadyClientRpc();
    }


    //[ClientRpc]
    //private void SetPlayerReadyClientRpc(ClientRpcParams clientRpcParams = default)
    //{
    //    Debug.Log(clientRpcParams.Send.TargetClientIds);
    //}

    public bool IsLocalPlayerReady() => isLocalPlayerReady;
   
    public void SetPlayMode(PlayMode playMode) => this.playMode = playMode;

    public PlayMode GetPlayMode() => playMode;
}
public enum PlayMode
{
    MultiplayerLocal, MultiplayerCom, MultiplayerOnline,
}
