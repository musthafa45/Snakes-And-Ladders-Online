using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileSetupUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputFieldPlayerName;
    [SerializeField] private Button okButton,exitButton;
    [SerializeField] private Transform playerNameSetupUiParent;


    private void Awake()
    {
        okButton.onClick.AddListener(() =>
        {
            PlayerProfileSetup.Instance.SetPlayerName(inputFieldPlayerName.text);
            Hide();
        });

        exitButton.onClick.AddListener(() => {
            Hide();
        });

        inputFieldPlayerName.onValueChanged.AddListener((value) =>
        {
            ValidatePlayerNameInputField();
        });

        ValidatePlayerNameInputField();
    }

    private void ValidatePlayerNameInputField()
    {
        okButton.interactable = inputFieldPlayerName.text != "" && inputFieldPlayerName.text.Length <= 10;
    }

    public void Show()
    {
        playerNameSetupUiParent.gameObject.SetActive(true);
    }

    public void Hide()
    {
        playerNameSetupUiParent.gameObject.SetActive(false);
    }
}
