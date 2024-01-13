using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProfileStatsHandlerUI : MonoBehaviour
{
    [SerializeField] private GameObject playersProfileStatsParent;
    [SerializeField] private List<PlayerProfileSingleUI> playerProfileSingleUIList;

    private void Start()
    {
        Hide();

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
        InitializePlayerSelectedProfileRevert(playerId);
    }
    private void OnDisable()
    {
        PlayerLocal.OnAnyPlayerSpawned -= PlayerLocal_OnAnyPlayerSpawned;
        PlayerLocal.OnPlayerReachedTargetTileWithPlayerId -= PlayerLocal_OnPlayerReachedTargetTileWithPlayerId;
    }

    private void InitializePlayerSelectedProfileRevert(short selectedPlayerId)
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
