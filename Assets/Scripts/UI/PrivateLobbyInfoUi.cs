using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrivateLobbyInfoUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameTextMeshProUGUI;
    [SerializeField] private TextMeshProUGUI lobbyCodeTextMeshProUGUI;

    [SerializeField] private Button copyCodeButton;

    private string lobbyCode;
    private void Awake()
    {
        copyCodeButton.onClick.AddListener(() =>
        {
            TextEditor textEditor = new TextEditor();
            textEditor.text = lobbyCode;
            textEditor.SelectAll();
            textEditor.Copy();

            Debug.Log("Code Copied");
        });
    }
    void Start()
    {
        SnakesAndLaddersLobby.Instance.OnPlayerCreatedPrivateLobby += SAL_lobby_Instance_OnPlayerCreatedPrivateLobby;

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

    private void SAL_lobby_Instance_OnPlayerCreatedPrivateLobby(string lobbyCode,string lobbyName)
    {
        Show();
        UpdateUi(lobbyCode, lobbyName);
    }

    private void UpdateUi(string lobbyCode, string lobbyName)
    {
        this.lobbyCode = lobbyCode;

        lobbyNameTextMeshProUGUI.text = "Lobby Name : " + lobbyName;
        lobbyCodeTextMeshProUGUI.text = "Lobby Code : " + lobbyCode;
    }

}
