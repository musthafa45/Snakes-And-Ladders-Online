using System;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WinLossUi : MonoBehaviour
{
    public static WinLossUi Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI winOrLossHeadText;
    [SerializeField] private TextMeshProUGUI matchNameText;
    [SerializeField] private TextMeshProUGUI winOrLossAmountText;
    [SerializeField] private Button okMenuButton;

    public bool IsMatchResultFinalized { get; private set; }
    private void Awake()
    {
        Instance = this;

        okMenuButton.onClick.AddListener(OnOkClicked);
    }

    private async void OnOkClicked() {
        okMenuButton.interactable = false;

        // Leave Lobby first
        if (SnakesAndLaddersLobby.Instance != null && NetworkManager.Singleton.IsClient) {
            await SnakesAndLaddersLobby.Instance.LeaveLobbyAsync();
        }

        if (NetworkManager.Singleton.IsHost) {
            await SnakesAndLaddersLobby.Instance.DeleteLobbyAsync();
        }

        NetworkManager.Singleton.Shutdown();
        // Load menu
        Loader.LoadScene(Loader.Scene.MainMenu);
    }

    private void Start()
    {
        if (SnakesAndLaddersMultiplayer.Instance != null) {
            SnakesAndLaddersMultiplayer.Instance.OnRemotePlayerDisconnected += SAL_Multiplayer_OnRemotePlayerDisconnected;
            SnakesAndLaddersMultiplayer.Instance.OnServerDisconnected += SAL_Multiplayer_OnServerDisconnected;
        }
        else {
            Debug.LogWarning("SnakesAndLaddersMultiplayer.Instance Not Found cant Sub");
        }

        if (UiManager.Instance != null) {
            UiManager.Instance.OnPlayerWonQuickmatch += UiManager_OnPlayerWonQuickmatch;
            UiManager.Instance.OnPlayerLossQuickMatch += UiManager_OnPlayerLossQuickMatch;

            UiManager.Instance.OnPlayerWonSelectLobbyMatch += UiManager_OnPlayerWonSelectLobbyMatch;
            UiManager.Instance.OnPlayerLossSelectLobbyMatch += UiManager_OnPlayerLossSelectLobbyMatch;
        }
        else {
            Debug.LogWarning("UiManager.Instance Not Found cant Sub");
        }
       
        Hide();

    }

    private void OnDestroy() {
        if (SnakesAndLaddersMultiplayer.Instance != null) {
            SnakesAndLaddersMultiplayer.Instance.OnRemotePlayerDisconnected -= SAL_Multiplayer_OnRemotePlayerDisconnected;
            SnakesAndLaddersMultiplayer.Instance.OnServerDisconnected += SAL_Multiplayer_OnServerDisconnected;
        }
        else {
            Debug.LogWarning("SnakesAndLaddersMultiplayer.Instance Not Found cant Unsub");
        }

        if (UiManager.Instance != null) {
            UiManager.Instance.OnPlayerWonQuickmatch -= UiManager_OnPlayerWonQuickmatch;
            UiManager.Instance.OnPlayerLossQuickMatch -= UiManager_OnPlayerLossQuickMatch;

            UiManager.Instance.OnPlayerWonSelectLobbyMatch -= UiManager_OnPlayerWonSelectLobbyMatch;
            UiManager.Instance.OnPlayerLossSelectLobbyMatch -= UiManager_OnPlayerLossSelectLobbyMatch;
        }
        else {
            Debug.LogWarning("UiManager.Instance Not Found cant Unsub");
        }
      
    }

    private void SAL_Multiplayer_OnRemotePlayerDisconnected(object sender, SnakesAndLaddersMultiplayer.OnPlayerDisConnectedEventArgs e) {

        if (NetworkManager.Singleton.LocalClientId == e.disconnectedClientId) {
            Debug.Log("Local Player Disconnected Show Loss UI");
        }
        else {
            Debug.Log("Other Player Disconnected Show Win UI");

            GameManager.LocalInstance.OnPlayerWin(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void SAL_Multiplayer_OnServerDisconnected(object sender, SnakesAndLaddersMultiplayer.OnPlayerDisConnectedEventArgs e) {
        UiManager.Instance.InvokeSelectLobbyWon();
    }

    private void UiManager_OnPlayerWonSelectLobbyMatch(object sender, UiManager.OnPlayerWonSelectLobbyMatchArgs e)
    {
        if (IsMatchResultFinalized) return;

        Show();

        float totalBetAmount = GetTotalBetAmountFromLobbyName(e.lobby.Name);
        PlayerWallet.AddCash(totalBetAmount);

        // Winner
        SetMessageHeadText("You Win!");
        SetMessageMatchName($"{e.lobby.Name}");
        SetMessageWinOrLossAmount($"+{totalBetAmount:N0}"); // N0 adds commas for thousands

        //Disable Dice buttons
        PlayerProfileStatsHandlerUI.Instance.DisableDiceAccess();

        IsMatchResultFinalized = true;
    }


    private void UiManager_OnPlayerLossSelectLobbyMatch(object sender, UiManager.OnPlayerLossSelectLobbyMatchArgs e)
    {
        if (IsMatchResultFinalized) return;

        Show();

        float entryAmount = GetEntryBetAmountFromLobbyName(e.lobby.Name);

        // Loser
        SetMessageHeadText("You Lost");
        SetMessageMatchName($"{e.lobby.Name}");
        SetMessageWinOrLossAmount($"-{entryAmount:N0}");

        //Disable Dice buttons
        PlayerProfileStatsHandlerUI.Instance.DisableDiceAccess();

        IsMatchResultFinalized = true;
    }

    private void UiManager_OnPlayerWonQuickmatch(object sender, System.EventArgs e)
    {
        if (IsMatchResultFinalized) return;

        Show();

        // Winner
        SetMessageHeadText("You Win!");
        SetMessageMatchName($"Quick Match");
        SetMessageWinOrLossAmount($"+{0:N0}"); // N0 adds commas for thousands

        //Disable Dice buttons
        PlayerProfileStatsHandlerUI.Instance.DisableDiceAccess();
    }

    private void UiManager_OnPlayerLossQuickMatch(object sender, System.EventArgs e)
    {
        if (IsMatchResultFinalized) return;

        Show();

        SetMessageHeadText("You Loss!");
        SetMessageMatchName($"Quick Match");
        SetMessageWinOrLossAmount($"+{0:N0}"); // N0 adds commas for thousands

        //Disable Dice buttons
        PlayerProfileStatsHandlerUI.Instance.DisableDiceAccess();
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
        return GameManager.LocalInstance.BetDataSO.BetDataSOList.Where(betData => betData.GameMode == matchName).FirstOrDefault().WinAmount;
    }

    private float GetEntryBetAmountFromLobbyName(string matchName)
    {
        return GameManager.LocalInstance.BetDataSO.BetDataSOList.Where(betData => betData.GameMode == matchName).FirstOrDefault().EntryAmount;
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
