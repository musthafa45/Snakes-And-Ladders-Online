using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerProfileStatsHandlerUI : MonoBehaviour
{
    public static PlayerProfileStatsHandlerUI Instance {  get; private set; }

    [SerializeField] private GameObject playersProfileStatsParent;
    [SerializeField] private PlayerProfileSingleUI player0ProfileSingleUI;
    [SerializeField] private PlayerProfileSingleUI player1ProfileSingleUI;

    private PlayerProfileSingleUI playerProfileLocalPlayer;
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

                SetLocalPlayerProfile(selectedPlayerId);
                DisableOpponentButtonAccess();

                InitializePlayerSelectedProfile(selectedPlayerId);
            };

            GameManager.LocalInstance.OnPlayerMovedSuccess += GameManager_LocalInstance_OnPlayerMovedSuccess;
        };

        //PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;
       
    }

    private void DisableOpponentButtonAccess()
    {
        if(playerProfileLocalPlayer == player0ProfileSingleUI)
        {
            player1ProfileSingleUI.ButtonInteractableEnabled(false);
        }
        else if(playerProfileLocalPlayer == player1ProfileSingleUI)
        {
            player0ProfileSingleUI.ButtonInteractableEnabled(false);
        }
    }

    private void SetLocalPlayerProfile(short selectedPlayerId)
    {
        playerProfileLocalPlayer = selectedPlayerId == 0 ? player0ProfileSingleUI : player1ProfileSingleUI;
    }

    private void GameManager_LocalInstance_OnPlayerMovedSuccess(short playerId)
    {
        InitializePlayerSelectedProfileRevert(playerId);
    }

    private void PlayerLocal_OnAnyPlayerSpawned(short playerId)
    {
        //PlayerLocal.OnAnyPlayerReachedTargetTile += PlayerLocal_OnPlayerReachedTargetTileWithPlayerId;
    }

    //private void PlayerLocal_OnPlayerReachedTargetTileWithPlayerId(short playerId)
    //{
        
    //}
    private void OnDisable()
    {
        PlayerLocal.OnAnyPlayerSpawned -= PlayerLocal_OnAnyPlayerSpawned;
        //PlayerLocal.OnAnyPlayerReachedTargetTile -= PlayerLocal_OnPlayerReachedTargetTileWithPlayerId;
        GameManager.LocalInstance.OnPlayerMovedSuccess -= GameManager_LocalInstance_OnPlayerMovedSuccess;
    }

    private void InitializePlayerSelectedProfile(short selectedPlayerId)
    {
        //for (int i = 0; i < playerProfileSingleUIList.Count; i++)
        //{
        //    if (playerProfileSingleUIList[i].GetPlayerConnectedId() == selectedPlayerId)
        //    {
        //        playerProfileSingleUIList[i].ButtonInteractableEnabled(true);
        //        playerProfileSingleUIList[i].ButtonAccessbilityCheck(); // Double Check Has Turn Access
        //    }
        //    else
        //    {
        //        playerProfileSingleUIList[i].ButtonInteractableEnabled(false);
        //    }
        //}

        if (selectedPlayerId == player0ProfileSingleUI.GetPlayerConnectedId())
        {
            player0ProfileSingleUI.ButtonInteractableEnabled(true);
           //player1ProfileSingleUI.ButtonInteractableEnabled(false);
        }
        else if (selectedPlayerId == player1ProfileSingleUI.GetPlayerConnectedId())
        {
            //player0ProfileSingleUI.ButtonInteractableEnabled(false);
            player1ProfileSingleUI.ButtonInteractableEnabled(true);
        }
    }
    private void InitializePlayerSelectedProfileRevert(short selectedPlayerId)
    {
        //for (int i = 0; i < playerProfileSingleUIList.Count; i++)
        //{
        //    if (playerProfileSingleUIList[i].GetPlayerConnectedId() == selectedPlayerId ||
        //        playerProfileSingleUIList[i].GetPlayerConnectedId() != (short)NetworkManager.Singleton.LocalClientId)
        //    {
        //        playerProfileSingleUIList[i].ButtonInteractableEnabled(false);

        //        if(playerProfileSingleUIList[i].GetPlayerConnectedId() != (short)NetworkManager.Singleton.LocalClientId)
        //        {
        //            playerProfileSingleUIList[i].UpdateSelectedVisual(true);
        //        }

        //    }
        //    else
        //    {
        //       playerProfileSingleUIList[i].ButtonInteractableEnabled(true);
        //    }
        //}

        if (selectedPlayerId == player0ProfileSingleUI.GetPlayerConnectedId())
        {
            player0ProfileSingleUI.ButtonInteractableEnabled(false);
            player1ProfileSingleUI.ButtonInteractableEnabled(true);
        }
        else if (selectedPlayerId == player1ProfileSingleUI.GetPlayerConnectedId())
        {
            player0ProfileSingleUI.ButtonInteractableEnabled(true);
            player1ProfileSingleUI.ButtonInteractableEnabled(false);
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
