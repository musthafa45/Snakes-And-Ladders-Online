using UnityEngine;

public class WinLossUi_PlayerVsCom : MonoBehaviour
{
    public static WinLossUi_PlayerVsCom Instance { get; private set; }

    [SerializeField] private Transform winUiParent,lossUiParent;

    private void Awake() {
        Instance = this;
    }

    public void ReloadScene() {
        // Load Current Scene
        Loader.LoadScene(Loader.Scene.PlayerVsComputerGame);
    }

    public void LoadMainMenu() {
        // Load menu
        Loader.LoadScene(Loader.Scene.MainMenu);
    }

    private void Start() {
        HideLossUi();
        HideWinUi();
    }

    public void HideWinUi() {
        winUiParent.gameObject.SetActive(false);
    }
    public void HideLossUi() {   
        lossUiParent.gameObject.SetActive(false);
    }

    public void ShowWinUi() {
        winUiParent.gameObject.SetActive(true);
    }
    public void ShowLossUi() {
        lossUiParent.gameObject.SetActive(true);
    }
}
