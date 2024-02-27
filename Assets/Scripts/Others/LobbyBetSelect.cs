using System;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBetSelect : MonoBehaviour
{
    public static LobbyBetSelect Instance { get; private set; }

    public event EventHandler<OnBetModifiedArgs> OnBetModified;

    public class OnBetModifiedArgs : EventArgs
    {
        public BetData selectedBet;
    }
    [SerializeField] private List<BetData> betDataList;

    private BetData currentSelectedBet;

    private void Awake()
    {
        Instance = this;

        currentSelectedBet = betDataList[0];
    }

    private void Start()
    {
        SelectLobbyUi.Instance.OnBetIncreaseClicked += SelectLobbyUi_OnBetIncreaseClicked;
        SelectLobbyUi.Instance.OnBetDecreaseClicked += SelectLobbyUi_OnBetDecreaseClicked;

        OnBetModified?.Invoke(this, new OnBetModifiedArgs
        {
            selectedBet = currentSelectedBet,
        });
    }

    private void SelectLobbyUi_OnBetDecreaseClicked(object sender, EventArgs e)
    {
        int previousIndex = betDataList.IndexOf(currentSelectedBet);
        previousIndex--;

        int clampedIndex = Mathf.Clamp(previousIndex, 0, betDataList.Count - 1);

        currentSelectedBet = betDataList[clampedIndex];
        
        OnBetModified?.Invoke(this, new OnBetModifiedArgs
        {
            selectedBet = currentSelectedBet,
        });

        Debug.Log("Current Bet Win "+ currentSelectedBet.WinAmount + "Current bet Entry " + currentSelectedBet.EntryAmount);
    }

    private void SelectLobbyUi_OnBetIncreaseClicked(object sender, EventArgs e)
    {
        int nextIndex = betDataList.IndexOf(currentSelectedBet);
        nextIndex++;

        int clampedIndex = Mathf.Clamp(nextIndex, 0, betDataList.Count - 1);

        currentSelectedBet = betDataList[clampedIndex];

        OnBetModified?.Invoke(this, new OnBetModifiedArgs
        {
            selectedBet = currentSelectedBet,
        });

        Debug.Log("Current Bet Win "+ currentSelectedBet.WinAmount + "Current bet Entry " + currentSelectedBet.EntryAmount);

    }

    [System.Serializable]
    public class BetData
    {
        public float WinAmount;
        public float EntryAmount;
    }
}
