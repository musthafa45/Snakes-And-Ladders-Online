using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerProfileStatsHandlerUI : NetworkBehaviour
{
    public static PlayerProfileStatsHandlerUI LocalInstance;
   
    [SerializeField] private GameObject playersProfileStatsParent;
    [SerializeField] private List<PlayerProfileSingleUI> playerProfileSingleUIList; // 0 is Local player, 1 is Player 2

    private void Awake()
    {
        LocalInstance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        Hide();

    }


    public void OnAnyPlayerMoveDone(ulong localClientId)
    {
        if(localClientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"Moved Client Id {localClientId} is Local Player");

            playerProfileSingleUIList[0].ButtonInteractableEnabled(false);
            playerProfileSingleUIList[0].SetSelectedVisual(false);

            playerProfileSingleUIList[1].SetSelectedVisual(true);
            playerProfileSingleUIList[1].ButtonInteractableEnabled(false);
        }
        else
        {
            Debug.Log($"Moved Client Id {localClientId} Opponent Player");
            playerProfileSingleUIList[0].ButtonInteractableEnabled(true);
            playerProfileSingleUIList[0].SetSelectedVisual(true);

            playerProfileSingleUIList[1].SetSelectedVisual(false);
            playerProfileSingleUIList[1].ButtonInteractableEnabled(false);
        }

    }

    public void InitializePlayerSelectedProfile(short selectedPlayerId)
    {
        if(selectedPlayerId == (short)NetworkManager.Singleton.LocalClientId)
        {
            //local Player Selected
            playerProfileSingleUIList[0].ButtonInteractableEnabled(true);
            playerProfileSingleUIList[0].SetSelectedVisual(true);

            playerProfileSingleUIList[1].ButtonInteractableEnabled(false);
            playerProfileSingleUIList[1].SetSelectedVisual(false);
        }
        else
        {
            //opponent Player selected
            playerProfileSingleUIList[0].ButtonInteractableEnabled(false);
            playerProfileSingleUIList[0].SetSelectedVisual(false);

            playerProfileSingleUIList[1].ButtonInteractableEnabled(false);
            playerProfileSingleUIList[1].SetSelectedVisual(true);
        }
    }

    public void SetupPlayerProfile(short selectedPlayerId)
    {
        Show();
        Debug.Log("Current Default Turn Player Id Is :" + selectedPlayerId);
        InitializePlayerSelectedProfile(selectedPlayerId);
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
