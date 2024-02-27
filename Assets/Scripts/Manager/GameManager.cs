using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager LocalInstance { get; private set; }

    public static event Action<GameManager> OnAnyGameManagerSpawned;

    [SerializeField] private Transform playerPrefab;

    private void Awake()
    {
        LocalInstance = this;
    }

    public override void OnNetworkSpawn()
    {
        PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;

        OnAnyGameManagerSpawned?.Invoke(this);

        if(IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }

        ulong selectedPlayerId = 0;
        if (IsServer)
        {
           selectedPlayerId = (ulong)UnityEngine.Random.Range(0, 2);
        }

        SelectRandomPlayerForFirstMoveServerRpc(selectedPlayerId);
        SetPlayerNamesServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNamesServerRpc()
    {
        SetPlayerNamesClientRpc();
    }

    [ClientRpc]
    private void SetPlayerNamesClientRpc()
    {
        PlayerProfileStatsHandlerUI.Instance.SetPlayerNames();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectRandomPlayerForFirstMoveServerRpc(ulong selectedPlayerId)
    {
        SelectRandomPlayerForFirstMoveClientRpc(selectedPlayerId);
    }

    [ClientRpc]
    private void SelectRandomPlayerForFirstMoveClientRpc(ulong selectedPlayerId)
    {
        PlayerProfileStatsHandlerUI.Instance.SetupPlayersRandomFirstMove(selectedPlayerId);
    }

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
        PlayerProfileStatsHandlerUI.Instance.OnAnyPlayerMoveDone(localClientId);
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

    private void PlayerLocal_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        PlayerLocal.OnAnyPlayerSpawned -= PlayerLocal_OnAnyPlayerSpawned;
        PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;
    }
}
