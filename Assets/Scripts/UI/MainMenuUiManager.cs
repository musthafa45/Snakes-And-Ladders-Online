using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] private Button muliplayerQuickMatchButton;
    [SerializeField] private Button muliplayerSelectLobbyButton;
    [SerializeField] private Button multiplayerLocalButton;
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

        multiplayerLocalButton.onClick.AddListener(() =>
        {
            
        });

        multiplayerComputerButton.onClick.AddListener(() =>
        {
            
        });

        settingsButton.onClick.AddListener(() => {
            settingsUi.gameObject.SetActive(true);
        });

    }


    private void Start() {
        versionTextmeshPro.text = $"Version: {Application.version}";
    }
}


