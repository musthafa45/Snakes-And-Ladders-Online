using System;
using UnityEngine;

public static class PlayerWallet
{
    private const string CASHAMOUNT_KEY = "cashAmount";

    public static event EventHandler<OnPlayerWalletModifiedArgs> OnPlayerWalletModified;
    public class OnPlayerWalletModifiedArgs : EventArgs
    {
        public float currentCashAmount;
    }
    private static float cashAmount = 2000;

    public static void AddCash(float amount)
    {
        cashAmount += amount;

        PlayerPrefs.SetFloat(CASHAMOUNT_KEY, cashAmount);

        OnPlayerWalletModified?.Invoke(null,new OnPlayerWalletModifiedArgs
        {
            currentCashAmount = amount
        });
    }

    public static void RemoveCash(float amount)
    {
        cashAmount -= amount;

        PlayerPrefs.SetFloat(CASHAMOUNT_KEY, cashAmount);

        OnPlayerWalletModified?.Invoke(null, new OnPlayerWalletModifiedArgs
        {
            currentCashAmount = amount
        });
    }

    public static float GetCurrentCashAmount()
    {
        if(PlayerPrefs.HasKey(CASHAMOUNT_KEY))
        {
            return PlayerPrefs.GetFloat(CASHAMOUNT_KEY);
        }
        else
        {
            return cashAmount;
        }
    }
}
