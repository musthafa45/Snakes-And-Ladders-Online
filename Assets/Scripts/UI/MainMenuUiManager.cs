using UnityEngine;
using UnityEngine.UI;
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
            Loader.LoadScene(Loader.Scene.QuickLobby);
        });

        muliplayerSelectLobbyButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.SelectLobby);
        });

        multiplayerLocalButton.onClick.AddListener(() =>
        {
            
        });

        multiplayerComputerButton.onClick.AddListener(() =>
        {
            
        });

    }
}
