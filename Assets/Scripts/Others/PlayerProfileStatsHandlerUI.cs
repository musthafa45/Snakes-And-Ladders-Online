using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerProfileStatsHandlerUI : MonoBehaviour
{
    public static PlayerProfileStatsHandlerUI Instance { get; private set; }

    [SerializeField] private GameObject playersProfileStatsParent;
    [SerializeField] private List<PlayerProfileSingleUI> playerProfileSingleUIList; // 0 is Local player, 1 is Opponet Player

    private void Awake()
    {
        Instance = this;
    }

    public void OnAnyPlayerMoveDone(ulong localClientId)
    {
        if (localClientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"Moved Player Client Id {localClientId} is Local Player now opponent Player Turn");

            playerProfileSingleUIList[0].ButtonInteractableEnabled(false);
            playerProfileSingleUIList[0].SetSelectedVisual(false);

            playerProfileSingleUIList[1].ButtonInteractableEnabled(false);
            playerProfileSingleUIList[1].SetSelectedVisual(true);
        }
        else
        {
            Debug.Log($"Moved Client Id {localClientId} Opponent Player now Local Player Turn");

            playerProfileSingleUIList[0].ButtonInteractableEnabled(true);
            playerProfileSingleUIList[0].SetSelectedVisual(true);

            playerProfileSingleUIList[1].SetSelectedVisual(false);
            playerProfileSingleUIList[1].ButtonInteractableEnabled(false);
        }

    }

    public void InitializePlayerSelectedProfile(ulong selectedPlayerId)
    {
        if (selectedPlayerId == NetworkManager.Singleton.LocalClientId)
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

    public void SetupPlayersRandomFirstMove(ulong selectedClientId)
    {
        InitializePlayerSelectedProfile(selectedClientId);
    }

    public void SetPlayerNames()
    {
        playerProfileSingleUIList[0].SetPlayerName(PlayerPrefs.GetString("PlayerName")); // local Player Profile
        playerProfileSingleUIList[1].SetPlayerName(GetOpponentPlayerName().ToString()); // Opponent Player Profile
    }

    private FixedString64Bytes GetOpponentPlayerName()
    {
        Lobby lobby = SnakesAndLaddersLobby.Instance.GetJoinedLobby();

        foreach(Player player in lobby.Players)
        {
            Debug.Log("Playing Player Name " + player.Data["PlayerName"].Value + "Player Client Id " + player.Id);
        }

        for (int i = 0; i < lobby.Players.Count; i++)
        {
            if (lobby.Players[i].Id != AuthenticationService.Instance.PlayerId)
            {
                // this is enemy
                return lobby.Players[i].Data["PlayerName"].Value;
            }

        }
        return string.Empty;
    }

    public void DoOpponentSpin(short selectedFaceValue) {
        playerProfileSingleUIList[1].GetDiceRollAnimation().PlayOpponentDiceRoll(selectedFaceValue);
    }
}
