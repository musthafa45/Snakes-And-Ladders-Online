using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNamesEnterUi : MonoBehaviour {
    [SerializeField] private TMP_InputField tmp_InputField_player1;
    [SerializeField] private TMP_InputField tmp_InputField_player2;
    [SerializeField] private Button okButton;
    [SerializeField] private Transform playerNamesSetupUiParent;

    private const int MaxNameLength = 10;

    private void Awake() {
        tmp_InputField_player1.onValueChanged.AddListener(_ => ValidateInputs());
        tmp_InputField_player2.onValueChanged.AddListener(_ => ValidateInputs());

        okButton.onClick.AddListener(()=> {
            Hide();
            PlayerProfileStatsHandlerUI.Instance.SetPlayerNames_PassAndPlay(tmp_InputField_player1.text, tmp_InputField_player2.text);
            GameManager_PassAndPlay.Instance.SetPlayerNames(tmp_InputField_player1.text, tmp_InputField_player2.text);
            GameManager_PassAndPlay.Instance.StartGame();
        });

        ValidateInputs();
        Show();
    }

    private void ValidateInputs() {
        bool isPlayer1Valid =
            !string.IsNullOrWhiteSpace(tmp_InputField_player1.text) &&
            tmp_InputField_player1.text.Length <= MaxNameLength;

        bool isPlayer2Valid =
            !string.IsNullOrWhiteSpace(tmp_InputField_player2.text) &&
            tmp_InputField_player2.text.Length <= MaxNameLength;

        okButton.interactable = isPlayer1Valid && isPlayer2Valid;
    }

    public void Show() {
        playerNamesSetupUiParent.gameObject.SetActive(true);
    }

    public void Hide() {
        playerNamesSetupUiParent.gameObject.SetActive(false);
    }
}
