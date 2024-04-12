using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLossUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winOrLossHeadText;
    [SerializeField] private TextMeshProUGUI matchNameText;
    [SerializeField] private TextMeshProUGUI winOrLossAmountText;
    [SerializeField] private Button mainMenuButton;
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SnakesAndLaddersLobby.Instance.LeaveLobby();
            SceneManager.LoadScene("MainMenu");
        });
    }

    private void Start()
    {
        SnakesAndLaddersMultiplayer.Instance.OnClientDisconnected += SAL_Multiplayer_OnClientDisconnected;

        UiManager.Instance.OnPlayerWonQuickmatch += UiManager_OnPlayerWonQuickmatch;
        UiManager.Instance.OnPlayerLossQuickMatch += UiManager_OnPlayerLossQuickMatch;

        UiManager.Instance.OnPlayerWonSelectLobbyMatch += UiManager_OnPlayerWonSelectLobbyMatch;
        UiManager.Instance.OnPlayerLossSelectLobbyMatch += UiManager_OnPlayerLossSelectLobbyMatch;
        Hide();

    }

    private void UiManager_OnPlayerWonSelectLobbyMatch(object sender, UiManager.OnPlayerWonSelectLobbyMatchArgs e)
    {
        Show();

        float totalBetAmount = GetTotalBetAmountFromLobbyName(e.lobby.Name);
        PlayerWallet.AddCash(totalBetAmount);

        SetMessageHeadText("You Won");
        SetMessageMatchName($"You Won {e.lobby.Name}");
        SetMessageWinOrLossAmount($"You Won {totalBetAmount}");
    }


    private void UiManager_OnPlayerLossSelectLobbyMatch(object sender, UiManager.OnPlayerLossSelectLobbyMatchArgs e)
    {
        Show();

        float entryAmount = GetEntryBetAmountFromLobbyName(e.lobby.Name);
        PlayerWallet.RemoveCash(entryAmount);

        SetMessageHeadText("You Loss");
        SetMessageMatchName($"You Loss {e.lobby.Name}");
        SetMessageWinOrLossAmount($"You Loss {entryAmount}");
    }

    private void UiManager_OnPlayerWonQuickmatch(object sender, System.EventArgs e)
    {
        Show();

        SetMessageHeadText("You Won");
        SetMessageMatchName("You Won Quick Match");
        SetMessageWinOrLossAmount("You Won 0 Amount");
    }

    private void UiManager_OnPlayerLossQuickMatch(object sender, System.EventArgs e)
    {
        Show();

        SetMessageHeadText("You Loss");
        SetMessageMatchName("You Loss Quick Match");
        SetMessageWinOrLossAmount("You Loss 0 Amount");
    }

    private void Hide()
    {
        ResetUi();

        gameObject.SetActive(false);
    }

    private void Show()
    {
        ResetUi();

        gameObject.SetActive(true);
    }

    private void ResetUi()
    {
        SetMessageHeadText(string.Empty);
        SetMessageMatchName(string.Empty);
        SetMessageWinOrLossAmount(string.Empty);
    }

    private float GetTotalBetAmountFromLobbyName(string matchName)
    {
        return LobbyBetSelect.Instance.BetDatas.Where(betData => betData.GameMode == matchName).FirstOrDefault().WinAmount;
    }

    private float GetEntryBetAmountFromLobbyName(string matchName)
    {
        return LobbyBetSelect.Instance.BetDatas.Where(betData => betData.GameMode == matchName).FirstOrDefault().EntryAmount;
    }

    private void SAL_Multiplayer_OnClientDisconnected(object sender, SnakesAndLaddersMultiplayer.OnClientDisconnectedArgs e)
    {
        Show();

        if(e.clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Local Client disConnected");
        }
        else
        {
            Debug.Log("Opponent disConnected");
        }
    }

    private void SetMessageHeadText(string message)
    {
       winOrLossHeadText.text = message;
    }

    private void SetMessageMatchName(string message)
    {
        matchNameText.text = message;
    }

    private void SetMessageWinOrLossAmount(string message)
    {
        winOrLossAmountText.text = message;
    }
}
