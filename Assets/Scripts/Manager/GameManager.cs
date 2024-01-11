using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager LocalInstance { get; private set; }

    public static event Action<GameManager> OnAnyGameManagerSpawned;
    public event EventHandler OnStartMatchPerformed;

    private Dictionary<ulong, bool> playerReadyDictionary;
    [SerializeField] private PlayMode playMode;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

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

        TestingNetCodeUI.Instance.OnPlayerClickedHostOrClientBtn += TestingNetCodeUI_OnPlayerClickedHostOrClientBtn;
        PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;

        OnAnyGameManagerSpawned?.Invoke(this);
    }

    public override void OnNetworkDespawn()
    {
        
    }

    private void PlayerLocal_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        SetPlayerReadyAndTryStartMatch();

        PlayerLocal.OnAnyPlayerSpawned -= PlayerLocal_OnAnyPlayerSpawned;
        PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;
    }

    private void TestingNetCodeUI_OnPlayerClickedHostOrClientBtn(object sender, EventArgs e)
    {
        Debug.Log("Clients Total " + NetworkManager.Singleton.ConnectedClients.Count);
        SetPlayerReadyServerRpc();
    }

    private void OnDisable()
    {
        TestingNetCodeUI.Instance.OnPlayerClickedHostOrClientBtn -= TestingNetCodeUI_OnPlayerClickedHostOrClientBtn;
    }

    public void SetPlayerReadyAndTryStartMatch()
    {
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

        CheckMatchCanBeStart(allClientsReady);
    }

    private void CheckMatchCanBeStart(bool allClientsReady)
    {
        if (allClientsReady && NetworkManager.Singleton.ConnectedClientsIds.Count == 2)
        {
            StartMatchClientRpc(NetworkManager.Singleton.ConnectedClientsIds[0],
                NetworkManager.Singleton.ConnectedClientsIds[1]);
        }
    }

    [ClientRpc]
    private void StartMatchClientRpc(ulong player1, ulong player2)
    {
        Debug.Log($"Start Match In Both in Player {player1} And {player2}");
        OnStartMatchPerformed?.Invoke(this, EventArgs.Empty);
    }

    public PlayMode GetPlayMode() => playMode;

}
public enum PlayMode
{
    MultiplayerLocal, MultiplayerCom, MultiplayerOnline,
}
