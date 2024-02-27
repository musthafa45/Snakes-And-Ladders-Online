using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] private Button muliplayerOnlineButton;
    [SerializeField] private Button multiplayerLocalButton;
    [SerializeField] private Button multiplayerComputerButton;

    private void Awake()
    {
        muliplayerOnlineButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Lobby");
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
