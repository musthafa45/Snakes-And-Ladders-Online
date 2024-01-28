using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager LocalInstance { get; private set; }

    public static event Action<GameManager> OnAnyGameManagerSpawned;

    public event Action<short> OnStartMatchPerformed; //With Client Id Selected

    private Dictionary<ulong, bool> playerReadyDictionary;
    [SerializeField] private PlayMode playMode;
    private short FirstMovePlayerId;

    private void Awake()
    {
        if(LocalInstance == null)
        {
            LocalInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(LocalInstance);
        }
    }
    public override void OnNetworkSpawn()
    {
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

        OnAnyGameManagerSpawned?.Invoke(this as GameManager);
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

        // To Start Match
        if (allClientsReady && NetworkManager.Singleton.ConnectedClientsIds.Count == 2)
        {
            if (IsServer)
            {
                FirstMovePlayerId = (short)UnityEngine.Random.Range(0, 2);
                Debug.Log("Current Selected Is  :" + FirstMovePlayerId);
            }

            StartMatchClientRpc(FirstMovePlayerId);
        }
    }

    [ClientRpc]
    private void StartMatchClientRpc(short firstMovePlayerId)
    {
        OnStartMatchPerformed?.Invoke(firstMovePlayerId);

        PlayerProfileStatsHandlerUI.LocalInstance.SetupPlayerProfile(firstMovePlayerId);
    }

    public PlayMode GetPlayMode() => playMode;

    public void SetPlayerReachedTarget(ulong localClientId)
    {
        Debug.Log($"Moved Done Client {localClientId}");
        AnyPlayerMovedServerRpc(localClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AnyPlayerMovedServerRpc(ulong localClientId)
    {
        AnyPlayerMovedClientRpc(localClientId);
    }

    [ClientRpc]
    private void AnyPlayerMovedClientRpc(ulong localClientId)
    {
        PlayerProfileStatsHandlerUI.LocalInstance.OnAnyPlayerMoveDone(localClientId);
    }

    public void OnPlayerWin(ulong localClientId)
    {
        SetPlayerWinServerRpc(localClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerWinServerRpc(ulong localClientId)
    {
        SetPlayerWinClientRpc(localClientId);
    }
    [ClientRpc]
    private void SetPlayerWinClientRpc(ulong winlocalClientId)
    {
        UiManager.Instance.ShowGameFinishedUi(winlocalClientId);
    }
}
public enum PlayMode
{
    MultiplayerLocal, MultiplayerCom, MultiplayerOnline,
}
