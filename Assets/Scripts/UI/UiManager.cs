using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    
    public static UiManager Instance {  get; private set; }

    public event EventHandler OnPlayerWonQuickmatch;
    public event EventHandler OnPlayerLossQuickMatch;

    public event EventHandler<OnPlayerWonSelectLobbyMatchArgs> OnPlayerWonSelectLobbyMatch;
    public event EventHandler<OnPlayerLossSelectLobbyMatchArgs> OnPlayerLossSelectLobbyMatch;

    public class OnPlayerWonSelectLobbyMatchArgs : EventArgs { public Lobby lobby; }
    public class OnPlayerLossSelectLobbyMatchArgs : EventArgs { public Lobby lobby; }


    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI LobbyNameText;
    private void Awake()
    {
        Instance = this;

        menuButton.onClick.AddListener(OnMenuClicked);
    }

    private async void OnMenuClicked() {
        menuButton.interactable = false;

        // Leave Lobby first
        if (SnakesAndLaddersLobby.Instance != null && NetworkManager.Singleton.IsClient) {
            await SnakesAndLaddersLobby.Instance.LeaveLobbyAsync();
        }

        if (NetworkManager.Singleton.IsHost) {
            await SnakesAndLaddersLobby.Instance.DeleteLobbyAsync();
        }

        NetworkManager.Singleton.Shutdown();
        // Load menu
        Loader.LoadScene(Loader.Scene.MainMenu);
    }

    private void Start()
    {
        SetLobbyName();
    }

    private void SetLobbyName()
    {
        LobbyNameText.text = SnakesAndLaddersLobby.Instance.GetJoinedLobby().Name;
    }

    public void ShowGameFinishedUi(ulong winnerClientId)
    {
        Debug.Log($"Winner Client id:- {winnerClientId} And Your Clent Id:- {NetworkManager.Singleton.LocalClientId}");

        if (GameManager.LocalInstance.LobbyType == SnakesAndLaddersLobby.LobbyType.QuickMatch)
        {
            if (winnerClientId == NetworkManager.Singleton.LocalClientId) // Local Player Won The Match)
            {
                Debug.Log("You Won Quick Match");
                OnPlayerWonQuickmatch?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Debug.Log("You Loss Quick Match");
                OnPlayerLossQuickMatch?.Invoke(this, EventArgs.Empty);
            }

        }
        else if (GameManager.LocalInstance.LobbyType == SnakesAndLaddersLobby.LobbyType.SelectLobby)
        {
            Lobby lobby = GameManager.LocalInstance.JoinedLobby;

            if (winnerClientId == NetworkManager.Singleton.LocalClientId) // Local Player Won The Match
            {
                Debug.Log("You Won Select Lobby Match " + lobby.Name);
                OnPlayerWonSelectLobbyMatch?.Invoke(this, new OnPlayerWonSelectLobbyMatchArgs { lobby = lobby });
            }
            else
            {
                Debug.Log("You Loss Select Lobby Match " + lobby.Name);
                OnPlayerLossSelectLobbyMatch?.Invoke(this,new OnPlayerLossSelectLobbyMatchArgs { lobby = lobby });
            }
           
        }
    }
}

