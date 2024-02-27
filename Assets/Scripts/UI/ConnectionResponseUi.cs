using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            SceneManager.LoadScene("MainMenu");
      
        });

        Hide();
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
        gameObject?.SetActive(false);
    }
}
