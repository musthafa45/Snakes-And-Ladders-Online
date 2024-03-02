using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinUi : MonoBehaviour
{
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
}
