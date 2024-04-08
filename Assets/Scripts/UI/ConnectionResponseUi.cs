using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseUi : MonoBehaviour
{
    public static ConnectionResponseUi Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI connectionMsgTextMeshProUGUI;
    [SerializeField] private Button okButton;


    private void Awake()
    {
        Instance = this;

        okButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.MainMenu);
        });
    }

    private void Start()
    {
        SnakesAndLaddersLobby.Instance.OnPrivateLobbyJoinFailed += SnakesAndLaddersLobby_OnPrivateLobbyJoinFailed;

        Hide();
    }

    private void SnakesAndLaddersLobby_OnPrivateLobbyJoinFailed(object sender, System.EventArgs e)
    {
        Show();
    }

    public void SetConnectionResponseMsg(string msg)
    {
        connectionMsgTextMeshProUGUI.text = msg;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
