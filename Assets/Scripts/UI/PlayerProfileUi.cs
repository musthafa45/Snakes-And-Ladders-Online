using TMPro;
using UnityEngine;

public class PlayerProfileUi : MonoBehaviour
{
    public static PlayerProfileUi Instance {  get; private set; }

    [SerializeField] private TextMeshProUGUI textMeshProUGUIplayerName;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializePlayerName();
    }

    private void InitializePlayerName()
    {
        textMeshProUGUIplayerName.text = PlayerPrefs.GetString("PlayerName");
    }
}
