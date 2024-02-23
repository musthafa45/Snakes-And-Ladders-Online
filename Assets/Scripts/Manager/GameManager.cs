using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager LocalInstance { get; private set; }

    public static event Action<GameManager> OnAnyGameManagerSpawned;

    [SerializeField] private PlayMode playMode;

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
        PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;

        OnAnyGameManagerSpawned?.Invoke(this);
    }

    private void PlayerLocal_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        PlayerLocal.OnAnyPlayerSpawned -= PlayerLocal_OnAnyPlayerSpawned;
        PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;
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
