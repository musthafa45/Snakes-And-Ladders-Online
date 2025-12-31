using System;
using UnityEngine;

public static class PlayerWallet {
    private const string CASH_AMOUNT_KEY = "cashAmount";
    private const float DEFAULT_CASH = 99999f;

    public static event EventHandler<OnPlayerWalletModifiedArgs> OnPlayerWalletModified;

    public class OnPlayerWalletModifiedArgs : EventArgs {
        public float currentCashAmount;
        public float changeAmount;
    }

    private static float? _cashAmount = null; // Nullable to track if loaded

    // Property that automatically loads from PlayerPrefs on first access
    private static float CashAmount {
        get {
            if (_cashAmount == null) {
                LoadCashAmount();
            }
            return _cashAmount.Value;
        }
        set {
            _cashAmount = value;
        }
    }

    private static void LoadCashAmount() {
        if (PlayerPrefs.HasKey(CASH_AMOUNT_KEY)) {
            _cashAmount = PlayerPrefs.GetFloat(CASH_AMOUNT_KEY);
            Debug.Log($"Loaded cash amount: {_cashAmount}");
        }
        else {
            _cashAmount = DEFAULT_CASH;
            SaveCashAmount();
            Debug.Log($"Initialized cash amount: {_cashAmount}");
        }
    }

    private static void SaveCashAmount() {
        PlayerPrefs.SetFloat(CASH_AMOUNT_KEY, CashAmount);
        PlayerPrefs.Save(); // Important: Force save to disk
    }

    public static bool AddCash(float amount) {
        if (amount <= 0) {
            Debug.LogWarning($"Cannot add negative or zero amount: {amount}");
            return false;
        }

        float previousAmount = CashAmount;
        CashAmount += amount;
        SaveCashAmount();

        Debug.Log($"Added {amount} cash. Total: {CashAmount}");

        OnPlayerWalletModified?.Invoke(null, new OnPlayerWalletModifiedArgs {
            currentCashAmount = CashAmount,
            changeAmount = amount
        });

        return true;
    }

    public static bool RemoveCash(float amount) {
        if (amount <= 0) {
            Debug.LogWarning($"Cannot remove negative or zero amount: {amount}");
            return false;
        }

        if (CashAmount < amount) {
            Debug.LogWarning($"Insufficient funds. Required: {amount}, Available: {CashAmount}");
            return false;
        }

        float previousAmount = CashAmount;
        CashAmount -= amount;
        SaveCashAmount();

        Debug.Log($"Removed {amount} cash. Total: {CashAmount}");

        OnPlayerWalletModified?.Invoke(null, new OnPlayerWalletModifiedArgs {
            currentCashAmount = CashAmount,
            changeAmount = -amount
        });

        return true;
    }

    public static float GetCurrentCashAmount() {
        return CashAmount; // Uses the property which auto-loads
    }

    public static bool HasEnoughCash(float amount) {
        return CashAmount >= amount;
    }

    // Optional: Reset wallet (useful for testing)
    public static void ResetWallet() {
        CashAmount = DEFAULT_CASH;
        SaveCashAmount();

        OnPlayerWalletModified?.Invoke(null, new OnPlayerWalletModifiedArgs {
            currentCashAmount = CashAmount,
            changeAmount = 0
        });

        Debug.Log("Wallet reset to default amount");
    }

    // Optional: Set cash directly (useful for cheats/testing)
    public static void SetCash(float amount) {
        if (amount < 0) {
            Debug.LogWarning("Cannot set negative cash amount");
            return;
        }

        CashAmount = amount;
        SaveCashAmount();

        OnPlayerWalletModified?.Invoke(null, new OnPlayerWalletModifiedArgs {
            currentCashAmount = CashAmount,
            changeAmount = 0
        });
    }
}