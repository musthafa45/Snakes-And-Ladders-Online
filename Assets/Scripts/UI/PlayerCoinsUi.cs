using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCoinsUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinTextMeshproGui;

    private void Start()
    {
        PlayerWallet.OnPlayerWalletModified += PlayerWallet_OnPlayerWalletModified;

        UpdateCashUi(PlayerWallet.GetCurrentCashAmount());
    }

    private void PlayerWallet_OnPlayerWalletModified(object sender, PlayerWallet.OnPlayerWalletModifiedArgs e)
    {
        UpdateCashUi(e.currentCashAmount);
    }

    private void UpdateCashUi(float cashAmount)
    {
        coinTextMeshproGui.text = cashAmount.ToString();
    }

    private void OnDestroy()
    {
        PlayerWallet.OnPlayerWalletModified -= PlayerWallet_OnPlayerWalletModified;
    }

}
