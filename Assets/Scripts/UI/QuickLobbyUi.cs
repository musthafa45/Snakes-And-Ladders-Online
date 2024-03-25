using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuickLobbyUi : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SnakesAndLaddersLobby.Instance.LeaveLobby();
            Loader.LoadScene(Loader.Scene.MainMenu);
        });
    }
}
