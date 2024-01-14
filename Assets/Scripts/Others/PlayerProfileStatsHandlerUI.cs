using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerProfileStatsHandlerUI : NetworkBehaviour
{
    [SerializeField] private GameObject playersProfileStatsParent;
    [SerializeField] private List<PlayerProfileSingleUI> playerProfileSingleUIList;

    private void Awake()
    {
        Hide();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        //Hide();

        GameManager.OnAnyGameManagerSpawned += (gameManager) =>
        {
            gameManager.OnStartMatchPerformed += (selectedPlayerId) =>
            {
                Show();
                Debug.Log("Current Default Turn Player Id Is :" + selectedPlayerId);
                InitializePlayerSelectedProfile(selectedPlayerId);
            };
        };

        PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;
    }

    private void PlayerLocal_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        PlayerLocal.OnPlayerReachedTargetTileWithPlayerId += PlayerLocal_OnPlayerReachedTargetTileWithPlayerId;
    }

    private void PlayerLocal_OnPlayerReachedTargetTileWithPlayerId(short playerId)
    {
        InitializePlayerSelectedProfileRevertServerRpc(playerId);
    }
    private void OnDisable()
    {
        PlayerLocal.OnAnyPlayerSpawned -= PlayerLocal_OnAnyPlayerSpawned;
        PlayerLocal.OnPlayerReachedTargetTileWithPlayerId -= PlayerLocal_OnPlayerReachedTargetTileWithPlayerId;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InitializePlayerSelectedProfileRevertServerRpc(short selectedPlayerId)
    {
        InitializePlayerSelectedProfileRevertClientRpc(selectedPlayerId);
    }

    [ClientRpc]
    private void InitializePlayerSelectedProfileRevertClientRpc(short selectedPlayerId)
    {
        for (int i = 0; i < playerProfileSingleUIList.Count; i++)
        {
            if (playerProfileSingleUIList[i].GetPlayerConnectedId() == selectedPlayerId)
            {
                playerProfileSingleUIList[i].ButtonInteractableEnabled(false);
                playerProfileSingleUIList[i].ButtonAccessbilityCheckRevert(); // Double Check Has Turn Access
            }
            else
            {
                playerProfileSingleUIList[i].ButtonInteractableEnabled(true);
            }
        }
    }

    private void InitializePlayerSelectedProfile(short selectedPlayerId)
    {
        for (int i = 0; i < playerProfileSingleUIList.Count; i++)
        {
            if (playerProfileSingleUIList[i].GetPlayerConnectedId() == selectedPlayerId)
            {
                playerProfileSingleUIList[i].ButtonInteractableEnabled(true);
                playerProfileSingleUIList[i].ButtonAccessbilityCheck(); // Double Check Has Turn Access
            }
            else
            {
                playerProfileSingleUIList[i].ButtonInteractableEnabled(false);
            }
        }
    }

    private void Show()
    {
        playersProfileStatsParent.SetActive(true);
    }

    private void Hide()
    {
        playersProfileStatsParent.SetActive(false);
    }
}
