using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileSetupUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputFieldPlayerName;
    [SerializeField] private Button okButton;
    [SerializeField] private Transform playerNameSetupUiParent;


    private void Awake()
    {
        okButton.onClick.AddListener(() =>
        {
            PlayerProfileSetup.Instance.SetPlayerName(inputFieldPlayerName.text);
            Hide();
        });
    }


    public void Show()
    {
        playerNameSetupUiParent.gameObject.SetActive(true);
    }

    public void Hide()
    {
        playerNameSetupUiParent.gameObject.SetActive(false);
    }

    private void Update()
    {
        okButton.interactable = inputFieldPlayerName.text != "";
    }
}
