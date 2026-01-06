using System;
using TMPro;
using UnityEngine;

public class WinLossPassAndPlayUi : MonoBehaviour
{
    public static WinLossPassAndPlayUi Instance { get; private set; }

    [SerializeField] private Transform winLossResultUiParent;
    [SerializeField] private TextMeshProUGUI winLossResultText;

    public void ReloadScene() {
        // Load Current Scene
        Loader.LoadScene(Loader.Scene.PlayerVsComputerGame);
    }

    public void LoadMainMenu() {
        // Load menu
        Loader.LoadScene(Loader.Scene.MainMenu);
    }

    public void ShowWinResult(short winnerPlayerId) {
        Show();
        string player1Name = GameManager_PassAndPlay.Instance.GetPlayerName1();
        string player2Name = GameManager_PassAndPlay.Instance.GetPlayerName2();

        winLossResultText.text =
            winnerPlayerId == 0
            ? $"{player1Name} won the match."
            : $"{player2Name} won the match.";

    }


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Hide();
    }

    public void Hide() {
        winLossResultUiParent.gameObject.SetActive(false);
    }

    public void Show() {
        winLossResultUiParent.gameObject.SetActive(true);
    }

}
