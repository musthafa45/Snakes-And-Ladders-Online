using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerProfileStatsHandlerUI : MonoBehaviour
{
    public static PlayerProfileStatsHandlerUI Instance {  get; private set; }

    [SerializeField] private GameObject playersProfileStatsParent;
    [SerializeField] private Transform playersProfileHolderTransform;
    [SerializeField] private GameObject player0profilePrefab,player1ProfilePrefab;

    //[SerializeField] private PlayerProfileSingleUI player0ProfileSingleUI;
    //[SerializeField] private PlayerProfileSingleUI player1ProfileSingleUI;

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

                //InitializePlayerSelectedProfile(selectedPlayerId);
            };

            //GameManager.LocalInstance.OnPlayerMovedSuccess += GameManager_LocalInstance_OnPlayerMovedSuccess;
        };

        PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;

        //PlayerLocal.OnAnyPlayerSpawned += PlayerLocal_OnAnyPlayerSpawned;
       
    }

    private void PlayerLocal_OnAnyPlayerSpawned(short spawnedPlayerId)
    {
        var prefab = Instantiate(player0profilePrefab);
        prefab.transform.SetParent(playersProfileHolderTransform);
        prefab.GetComponent<NetworkObject>().Spawn(true);
    }

    //private void GameManager_LocalInstance_OnPlayerMovedSuccess(short playerId)
    //{
    //    InitializePlayerSelectedProfileRevert(playerId);
    //}

    //private void PlayerLocal_OnAnyPlayerSpawned(short playerId)
    //{
    //    //PlayerLocal.OnAnyPlayerReachedTargetTile += PlayerLocal_OnPlayerReachedTargetTileWithPlayerId;
    //}

    //private void PlayerLocal_OnPlayerReachedTargetTileWithPlayerId(short playerId)
    //{

    //}
    private void OnDisable()
    {
          PlayerLocal.OnAnyPlayerSpawned -= PlayerLocal_OnAnyPlayerSpawned;
        //PlayerLocal.OnAnyPlayerReachedTargetTile -= PlayerLocal_OnPlayerReachedTargetTileWithPlayerId;
        //GameManager.LocalInstance.OnPlayerMovedSuccess -= GameManager_LocalInstance_OnPlayerMovedSuccess;
    }

    //private void InitializePlayerSelectedProfile(short selectedPlayerId)
    //{
    //    if (selectedPlayerId == player0ProfileSingleUI.PlayerConnectedId)
    //    {
    //        player0ProfileSingleUI.ButtonInteractableEnabled(true);
    //        player1ProfileSingleUI.ButtonInteractableEnabled(false);
    //    }
    //    else if (selectedPlayerId == player1ProfileSingleUI.PlayerConnectedId)
    //    {
    //        player0ProfileSingleUI.ButtonInteractableEnabled(false);
    //        player1ProfileSingleUI.ButtonInteractableEnabled(true);
    //    }
    //}
    //private void InitializePlayerSelectedProfileRevert(short selectedPlayerId)
    //{
    //    if (selectedPlayerId == player0ProfileSingleUI.PlayerConnectedId)
    //    {
    //        player0ProfileSingleUI.ButtonInteractableEnabled(false);
    //        player1ProfileSingleUI.ButtonInteractableEnabled(true);
            
    //    }
    //    else if (selectedPlayerId == player1ProfileSingleUI.PlayerConnectedId)
    //    {
    //        player0ProfileSingleUI.ButtonInteractableEnabled(true);
    //        player1ProfileSingleUI.ButtonInteractableEnabled(false);
    //    }
    //}

    private void Show()
    {
        playersProfileStatsParent.SetActive(true);
    }

    private void Hide()
    {
        playersProfileStatsParent.SetActive(false);
    }
}
