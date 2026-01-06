using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ExitConfirmUi : MonoBehaviour
{
    [SerializeField] private Button okButton,exitCloseButton,menuButton;

    private void Awake() {
        okButton.onClick.AddListener(OnOkButtonClicked);

        exitCloseButton.onClick.AddListener(() => {
            gameObject.SetActive(false);
            menuButton.gameObject.SetActive(true);
        });
    }

    private async void OnOkButtonClicked() {
        okButton.interactable = false;

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
