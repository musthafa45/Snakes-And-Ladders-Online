using System;
using UnityEngine;

public class LobbyBetSelect : MonoBehaviour
{
    public static LobbyBetSelect Instance { get; private set; }

    public event EventHandler<OnBetModifiedArgs> OnBetModified;

    public class OnBetModifiedArgs : EventArgs
    {
        public BetDataSO.BetData selectedBet;
    }

    private BetDataSO.BetData currentSelectedBet;

    public BetDataSO BetDataSO => betDataSO;

    [SerializeField] private BetDataSO betDataSO;

    private void Awake()
    {
        Instance = this;

        currentSelectedBet = betDataSO.BetDataSOList[0];
    }

    private void Start()
    {
        SelectLobbyUi.Instance.OnBetIncreaseClicked += SelectLobbyUi_OnBetIncreaseClicked;
        SelectLobbyUi.Instance.OnBetDecreaseClicked += SelectLobbyUi_OnBetDecreaseClicked;

        PrivateLobbyUi.Instance.OnBetIncreaseClicked += PrivateLobbyUi_Instance_OnBetIncreaseClicked;
        PrivateLobbyUi.Instance.OnBetDecreaseClicked += PrivateLobbyUi_Instance_OnBetDecreaseClicked;
    }

    private void PrivateLobbyUi_Instance_OnBetDecreaseClicked(object sender, EventArgs e)
    {
        BetDecreamentOne();
    }

    private void PrivateLobbyUi_Instance_OnBetIncreaseClicked(object sender, EventArgs e)
    {
        BetIncreamentOne();
    }

    private void SelectLobbyUi_OnBetDecreaseClicked(object sender, EventArgs e)
    {
        BetDecreamentOne();

        //Debug.Log("Current Bet Win "+ currentSelectedBet.WinAmount + "Current bet Entry " + currentSelectedBet.EntryAmount);
    }


    private void SelectLobbyUi_OnBetIncreaseClicked(object sender, EventArgs e)
    {
        BetIncreamentOne();

        //Debug.Log("Current Bet Win "+ currentSelectedBet.WinAmount + "Current bet Entry " + currentSelectedBet.EntryAmount);

    }

    private void BetIncreamentOne()
    {
        int nextIndex = betDataSO.BetDataSOList.IndexOf(currentSelectedBet);
        nextIndex++;

        int clampedIndex = Mathf.Clamp(nextIndex, 0, betDataSO.BetDataSOList.Count - 1);

        currentSelectedBet = betDataSO.BetDataSOList[clampedIndex];

        OnBetModified?.Invoke(this, new OnBetModifiedArgs
        {
            selectedBet = currentSelectedBet,
        });
    }
    private void BetDecreamentOne()
    {
        int previousIndex = betDataSO.BetDataSOList.IndexOf(currentSelectedBet);
        previousIndex--;

        int clampedIndex = Mathf.Clamp(previousIndex, 0, betDataSO.BetDataSOList.Count - 1);

        currentSelectedBet = betDataSO.BetDataSOList[clampedIndex];

        OnBetModified?.Invoke(this, new OnBetModifiedArgs
        {
            selectedBet = currentSelectedBet,
        });
    }

}
