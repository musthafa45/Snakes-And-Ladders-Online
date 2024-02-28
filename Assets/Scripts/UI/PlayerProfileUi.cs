using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileUi : MonoBehaviour
{
    public static PlayerProfileUi Instance {  get; private set; }

    [SerializeField] private TextMeshProUGUI textMeshProUGUIplayerName;
    [SerializeField] private Button editNameButton;

    [SerializeField] private PlayerProfileSetupUI playerProfileSetupUI;

    private void Awake()
    {
        Instance = this;

        editNameButton.onClick.AddListener(() =>
        {
            playerProfileSetupUI.gameObject.SetActive(true);
        });
    }

    private void Start()
    {
        PlayerProfileSetup.Instance.OnPlayerNameModified += PlayerProfileSetup_OnPlayerNameModified;

        InitializePlayerName();
    }

    private void PlayerProfileSetup_OnPlayerNameModified()
    {
        InitializePlayerName();
    }

    private void InitializePlayerName()
    {
        textMeshProUGUIplayerName.text = PlayerPrefs.GetString("PlayerName");
    }

    private void OnDestroy()
    {
        PlayerProfileSetup.Instance.OnPlayerNameModified -= PlayerProfileSetup_OnPlayerNameModified;
    }
}
