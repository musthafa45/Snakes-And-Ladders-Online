using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerTurnController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        //if (!IsOwner) return;

        PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;
       
    }

    private void PlayerLocal_OnAnyPlayerSpawned(short playerId)
    {
        Debug.Log("Player Spawned");
        PlayerLocal.OnAnyPlayerSpawned -= PlayerLocal_OnAnyPlayerSpawned;
        PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;
    }

    //public override void OnNetworkDespawn()
    //{
    //    PlayerLocal.OnAnyPlayerSpawned -= PlayerLocal_OnAnyPlayerSpawned;
    //    PlayerLocal.OnPlayerReachedTargetTileWithPlayerId -= PlayerLocal_OnPlayerReachedTargetTileWithPlayerId1;
    //}
}
