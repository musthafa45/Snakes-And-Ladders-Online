using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrivateLobbyInfoUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameTextMeshProUGUI;
    [SerializeField] private TextMeshProUGUI lobbyCodeTextMeshProUGUI;
    void Start()
    {
        SnakesAndLaddersLobby.Instance.OnPlayerCreatedPrivateLobby += SAL_lobby_Instance_OnPlayerCreatedPrivateLobby;

        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void SAL_lobby_Instance_OnPlayerCreatedPrivateLobby(string lobbyCode,string lobbyName)
    {
        Show();
        UpdateUi(lobbyCode, lobbyName);
    }

    private void UpdateUi(string lobbyCode, string lobbyName)
    {
        lobbyNameTextMeshProUGUI.text = "Lobby Name : " + lobbyName;
        lobbyCodeTextMeshProUGUI.text = "Lobby Code : " + lobbyCode;
    }

}
