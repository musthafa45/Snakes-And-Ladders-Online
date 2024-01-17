using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProfileStatsHandlerUI : MonoBehaviour
{
    public static PlayerProfileStatsHandlerUI Instance {  get; private set; }

    [SerializeField] private GameObject playersProfileStatsParent;
    [SerializeField] private List<PlayerProfileSingleUI> playerProfileSingleUIList;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Hide();

        GameManager.OnAnyGameManagerSpawned += (localGameManager) =>
        {
            localGameManager.OnStartMatchPerformed += (selectedPlayerId) =>
            {
                Show();
                Debug.Log("Current Default Turn Player Id Is :" + selectedPlayerId);
                InitializePlayerSelectedProfile(selectedPlayerId);
            };
        };

        //PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;
    }

    private void PlayerLocal_OnAnyPlayerSpawned(short playerId)
    {
        PlayerLocal.OnAnyPlayerReachedTargetTile += PlayerLocal_OnPlayerReachedTargetTileWithPlayerId;
    }

    private void PlayerLocal_OnPlayerReachedTargetTileWithPlayerId(short playerId)
    {
        InitializePlayerSelectedProfileRevert(playerId);
    }
    private void OnDisable()
    {
        PlayerLocal.OnAnyPlayerSpawned -= PlayerLocal_OnAnyPlayerSpawned;
        PlayerLocal.OnAnyPlayerReachedTargetTile -= PlayerLocal_OnPlayerReachedTargetTileWithPlayerId;
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

    private void Show()
    {
        playersProfileStatsParent.SetActive(true);
    }

    private void Hide()
    {
        playersProfileStatsParent.SetActive(false);
    }
}
