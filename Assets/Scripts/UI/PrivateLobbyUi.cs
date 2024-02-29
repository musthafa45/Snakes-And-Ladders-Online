using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrivateLobbyUi : MonoBehaviour
{
    public static PrivateLobbyUi Instance {  get; private set; }

    public event EventHandler OnBetIncreaseClicked;
    public event EventHandler OnBetDecreaseClicked;

    public event EventHandler<OnPlayPrivateLobbyCreateClickedArgs> OnPlayPrivateLobbyCreateClicked;
    public class OnPlayPrivateLobbyCreateClickedArgs : EventArgs
    {
        public LobbyBetSelect.BetData betData;
    }

    [SerializeField] private Button CreateLobbyButton;
    [SerializeField] private Button betIncreaseBtn, betDecreaseBtn;

    [SerializeField] private TextMeshProUGUI winAmountTextMeshProUGUI;
    [SerializeField] private TextMeshProUGUI entryAmountTextMeshProUGUI;
    [SerializeField] private TMP_InputField lobbyNameInputField;

    private LobbyBetSelect.BetData currentBetData;

    [SerializeField] private Button createLobbyMenuButton;
    [SerializeField] private Button joinLobbyMenuButton;

    [SerializeField] private Transform createLobbyMenuParent;
    [SerializeField] private Transform joinLobbyMenuParent;

    [SerializeField] private Transform lobbySelectionUiParent;

    private void Awake()
    {
        Instance = this;

        createLobbyMenuButton.onClick.AddListener(() =>
        {
            createLobbyMenuParent.gameObject.SetActive(true);
            joinLobbyMenuParent.gameObject.SetActive(false);
        });

        joinLobbyMenuButton.onClick.AddListener(() =>
        {
            createLobbyMenuParent.gameObject.SetActive(false);
            joinLobbyMenuParent.gameObject.SetActive(true);
        });

    }

    private void Start()
    {
        LobbyBetSelect.Instance.OnBetModified += LobbyBetSelect_OnBetModified;

        CreateLobbyButton.onClick.AddListener(() =>
        {
            OnPlayPrivateLobbyCreateClicked?.Invoke(this, new OnPlayPrivateLobbyCreateClickedArgs
            {
                betData = new LobbyBetSelect.BetData
                {
                    WinAmount = currentBetData.WinAmount,
                    GameMode = lobbyNameInputField.text,
                    EntryAmount = currentBetData.EntryAmount,
                }
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

    private void Hide()
    {
        lobbySelectionUiParent.gameObject.SetActive(false);
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

        CreateLobbyButton.interactable = IsPlayerHasSufficiantEntryAmount(betData.EntryAmount);
    }

    private bool IsPlayerHasSufficiantEntryAmount(float entryAmount)
    {
        return PlayerWallet.GetCurrentCashAmount() >= entryAmount;
    }

}
