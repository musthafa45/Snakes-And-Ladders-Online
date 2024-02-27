using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] private Button muliplayerQuickMatchButton;
    [SerializeField] private Button muliplayerSelectLobbyButton;
    [SerializeField] private Button multiplayerLocalButton;
    [SerializeField] private Button multiplayerComputerButton;

    private void Awake()
    {
        muliplayerQuickMatchButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("QuickLobby");
        });

        muliplayerSelectLobbyButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SelectLobby");
        });

        multiplayerLocalButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MultiplayerLocalAndCom");
        });

        multiplayerComputerButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MultiplayerLocalAndCom");
        });

    }
}
