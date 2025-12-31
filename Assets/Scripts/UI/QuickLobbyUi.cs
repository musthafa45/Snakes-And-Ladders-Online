using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class QuickLobbyUi : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnMenuClicked);
    }

    private async void OnMenuClicked() {
        mainMenuButton.interactable = false;

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
}
