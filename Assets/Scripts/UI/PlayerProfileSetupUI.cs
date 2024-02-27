using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileSetupUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputFieldPlayerName;
    [SerializeField] private Button okButton;

    private void Awake()
    {
        okButton.onClick.AddListener(() =>
        {
            PlayerProfileSetup.Instance.SetPlayerName(inputFieldPlayerName.text);
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        okButton.interactable = inputFieldPlayerName.text != "";
    }
}
