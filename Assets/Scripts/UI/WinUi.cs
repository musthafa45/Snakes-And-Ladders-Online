using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winMessageText;
    [SerializeField] private Button mainMenuButton;
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SnakesAndLaddersLobby.Instance.LeaveLobby();
            SceneManager.LoadScene("MainMenu");
        });
    }

    private void Start()
    {
        SnakesAndLaddersMultiplayer.Instance.OnClientDisconnected += SAL_Multiplayer_OnClientDisconnected;

        UiManager.Instance.OnPlayerWonQuickmatch += UiManager_OnPlayerWonQuickmatch;
        UiManager.Instance.OnPlayerLossQuickMatch += UiManager_OnPlayerLossQuickMatch;

        UiManager.Instance.OnPlayerWonSelectLobbyMatch += UiManager_OnPlayerWonSelectLobbyMatch;
        UiManager.Instance.OnPlayerLossSelectLobbyMatch += UiManager_OnPlayerLossSelectLobbyMatch;
        Hide();

    }

    private void UiManager_OnPlayerWonSelectLobbyMatch(object sender, UiManager.OnPlayerWonSelectLobbyMatchArgs e)
    {
        Show();

        SetMessage($"You Won {e.lobby.Name} Match");
    }

    private void UiManager_OnPlayerLossSelectLobbyMatch(object sender, UiManager.OnPlayerLossSelectLobbyMatchArgs e)
    {
        Show();

        SetMessage($"You Loss {e.lobby.Name} Match");
    }

    private void UiManager_OnPlayerWonQuickmatch(object sender, System.EventArgs e)
    {
        Show();

        SetMessage($"You Won Quick Match");
    }

    private void UiManager_OnPlayerLossQuickMatch(object sender, System.EventArgs e)
    {
        Show();

        SetMessage($"You Loss Quick Match");
    }

    private void Hide()
    {
        SetMessage("");
        gameObject.SetActive(false);
    }

    private void Show()
    {
        SetMessage("");
        gameObject.SetActive(true);
    }

    private void SAL_Multiplayer_OnClientDisconnected(object sender, SnakesAndLaddersMultiplayer.OnClientDisconnectedArgs e)
    {
        Show();

        if(e.clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Local Client disConnected");
        }
        else
        {
            Debug.Log("Opponent disConnected");
        }
    }

    private void SetMessage(string message)
    {
        winMessageText.text = message;
    }
}
