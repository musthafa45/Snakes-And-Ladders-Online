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
    }

    private void InitializePlayerSelectedProfile(short selectedPlayerId)
    {
        for (int i = 0; i < playerProfileSingleUIList.Count; i++)
        {
            if (playerProfileSingleUIList[i].GetPlayerConnectedId() == selectedPlayerId)
            {
                playerProfileSingleUIList[i].ButtonInteractableEnabled(true);
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
