using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<bool> OnLocalPlayerReadyChanged;

    [SerializeField] private PlayMode playMode;
    private bool isLocalPlayerReady = false;

    private void Awake()
    {
        Instance = this;

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
        if (!IsOwner) return;

        isLocalPlayerReady = true;
        OnLocalPlayerReadyChanged?.Invoke(isLocalPlayerReady);

        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log(serverRpcParams.Receive.SenderClientId);
        SetPlayerReadyClientRpc();
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log(clientRpcParams.Send.TargetClientIds);
    }

    public bool IsLocalPlayerReady() => isLocalPlayerReady;
   
    public void SetPlayMode(PlayMode playMode) => this.playMode = playMode;

    public PlayMode GetPlayMode() => playMode;
}
public enum PlayMode
{
    MultiplayerLocal, MultiplayerCom, MultiplayerOnline,
}
