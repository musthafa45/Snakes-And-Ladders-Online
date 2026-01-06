using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] private Button muliplayerQuickMatchButton;
    [SerializeField] private Button muliplayerSelectLobbyButton;
    [SerializeField] private Button multiplayerPassAndPlayButton;
    [SerializeField] private Button multiplayerComputerButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private TextMeshProUGUI versionTextmeshPro;
    [SerializeField] private Transform settingsUi;
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

        multiplayerComputerButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.PlayerVsComputerGame);
        });

        multiplayerPassAndPlayButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.PassAndPlayGame);
        });

        settingsButton.onClick.AddListener(() => {
            settingsUi.gameObject.SetActive(true);
        });

    }


    private void Start() {
        versionTextmeshPro.text = $"Version: {Application.version}";
    }
}


