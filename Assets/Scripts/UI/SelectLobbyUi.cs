using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectLobbyUi : MonoBehaviour
{
    public static SelectLobbyUi Instance { get; private set; }

    public event EventHandler OnBetIncreaseClicked;
    public event EventHandler OnBetDecreaseClicked;

    [SerializeField] private TextMeshProUGUI availableCoinTextMeshProUGUI;
    [SerializeField] private Button playButton;
    [SerializeField] private Button betIncreaseBtn,betDecreaseBtn;

    [Space]
    [SerializeField] private TextMeshProUGUI winAmountTextMeshProUGUI;
    [SerializeField] private TextMeshProUGUI entryAmountTextMeshProUGUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerWallet.OnPlayerWalletModified += PlayerWallet_OnPlayerWalletModified;
        LobbyBetSelect.Instance.OnBetModified += LobbyBetSelect_OnBetModified;

        UpdatePlayerWalletUi();

        playButton.onClick.AddListener(() =>
        {

        });

        betIncreaseBtn.onClick.AddListener(() =>
        {
            OnBetIncreaseClicked?.Invoke(this, EventArgs.Empty);
        });

        betDecreaseBtn.onClick.AddListener(() =>
        {
            OnBetDecreaseClicked?.Invoke(this, EventArgs.Empty); 
        });
    }

    private void LobbyBetSelect_OnBetModified(object sender, LobbyBetSelect.OnBetModifiedArgs e)
    {
        UpdateBetUi(e.selectedBet);
    }

    private void UpdateBetUi(LobbyBetSelect.BetData betData)
    {
        winAmountTextMeshProUGUI.text = betData.WinAmount.ToString();
        entryAmountTextMeshProUGUI.text = "Entry: " + betData.EntryAmount.ToString();
    }

    private void PlayerWallet_OnPlayerWalletModified(object sender, PlayerWallet.OnPlayerWalletModifiedArgs e)
    {
        UpdatePlayerWalletUi();
    }

    private void UpdatePlayerWalletUi()
    {
        availableCoinTextMeshProUGUI.text = PlayerWallet.GetCurrentCashAmount().ToString();
    }
}
