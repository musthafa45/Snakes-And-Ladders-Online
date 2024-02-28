using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectLobbyUi : MonoBehaviour
{
    public static SelectLobbyUi Instance { get; private set; }

    public event EventHandler OnBetIncreaseClicked;
    public event EventHandler OnBetDecreaseClicked;

    public event EventHandler<OnPlayButtonClickedArgs> OnPlayButtonClicked;
    public class OnPlayButtonClickedArgs : EventArgs
    {
        public LobbyBetSelect.BetData betData;
    }

    [SerializeField] private TextMeshProUGUI availableCoinTextMeshProUGUI;
    [SerializeField] private Button playButton;
    [SerializeField] private Button betIncreaseBtn,betDecreaseBtn;

    [Space]
    [SerializeField] private TextMeshProUGUI winAmountTextMeshProUGUI;
    [SerializeField] private TextMeshProUGUI entryAmountTextMeshProUGUI;

    private LobbyBetSelect.BetData currentBetData;

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
            OnPlayButtonClicked?.Invoke(this, new OnPlayButtonClickedArgs
            {
                betData = currentBetData
            });

            Hide();
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
        currentBetData = e.selectedBet;

        UpdateBetUi(e.selectedBet);
    }

    private void UpdateBetUi(LobbyBetSelect.BetData betData)
    {
        winAmountTextMeshProUGUI.text = betData.WinAmount.ToString();
        entryAmountTextMeshProUGUI.text = "Entry: " + betData.EntryAmount.ToString();

        playButton.interactable = IsPlayerHasSufficiantEntryAmount(betData.EntryAmount);
    }

    private bool IsPlayerHasSufficiantEntryAmount(float entryAmount)
    {
        return PlayerWallet.GetCurrentCashAmount() >= entryAmount;
    }

    private void PlayerWallet_OnPlayerWalletModified(object sender, PlayerWallet.OnPlayerWalletModifiedArgs e)
    {
        UpdatePlayerWalletUi();
    }

    private void UpdatePlayerWalletUi()
    {
        availableCoinTextMeshProUGUI.text = PlayerWallet.GetCurrentCashAmount().ToString();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
