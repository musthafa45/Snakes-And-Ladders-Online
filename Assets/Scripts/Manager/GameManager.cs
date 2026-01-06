using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager LocalInstance { get; private set; }

    public static event Action<GameManager> OnAnyGameManagerSpawned;

    public BetDataSO BetDataSO => betDataSO;

    [SerializeField] private Transform playerPrefab;
    [SerializeField] private BetDataSO betDataSO;

    public SnakesAndLaddersLobby.LobbyType LobbyType;
    public Lobby JoinedLobby = null;

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
        DetuctEntryFeesServerRpc();
        InitializeGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void InitializeGameServerRpc()
    {
       InitializeGameClientRpc();
    }

    [ClientRpc]
    private void InitializeGameClientRpc()
    {
        LobbyType = SnakesAndLaddersLobby.Instance.GetLobbyType();
        JoinedLobby = SnakesAndLaddersLobby.Instance.GetJoinedLobby();

        //SnakesAndLaddersLobby.Instance.DeleteLobby();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DetuctEntryFeesServerRpc()
    {
       DetuctEntryFeesClientRpc();
    }

    [ClientRpc]
    private void DetuctEntryFeesClientRpc()
    {
        Lobby lobby = SnakesAndLaddersLobby.Instance.GetJoinedLobby();
        float entryMatchAmount = GetEntryBetAmountFromLobbyName(lobby.Name);
        PlayerWallet.RemoveCash(entryMatchAmount);
    }

    private float GetEntryBetAmountFromLobbyName(string matchName)
    {
        return betDataSO.BetDataSOList.Where(betData => betData.GameMode == matchName).FirstOrDefault().EntryAmount;
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

    public void OnPlayerWin(ulong winnerClientId)
    {
        SetPlayerWinServerRpc(winnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerWinServerRpc(ulong winnerClientId)
    {
        SetPlayerWinClientRpc(winnerClientId);
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

    [ServerRpc(RequireOwnership = false)]
    public void SendDiceValueToOpponentServerRpc(
    short diceFaceValue,
    ServerRpcParams serverRpcParams = default) {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (clientId != senderClientId) {
                SendDiceValueToOpponentClientRpc(
                    diceFaceValue,
                    new ClientRpcParams {
                        Send = new ClientRpcSendParams {
                            TargetClientIds = new[] { clientId }
                        }
                    }
                );
                break;
            }
        }
    }

    [ClientRpc]
    private void SendDiceValueToOpponentClientRpc(short diceFaceValue, ClientRpcParams clientRpcParams = default) {
        // Run opponent-side animation with SAME value
        PlayerProfileStatsHandlerUI.Instance.DoOpponentSpin(diceFaceValue);
    }

    private async void OnApplicationQuit() {
        if (SnakesAndLaddersLobby.Instance != null) {
            await SnakesAndLaddersLobby.Instance.LeaveLobbyAsync();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RollTimeoutServerRpc(ServerRpcParams rpcParams = default) {
        ulong timedOutClientId = rpcParams.Receive.SenderClientId;

        ulong winnerClientId = GetOtherPlayerId(timedOutClientId);

        SetPlayerWinServerRpc(winnerClientId);
    }


    private ulong GetOtherPlayerId(ulong localId) {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (clientId != localId)
                return clientId;
        }
        return localId;
    }

}
