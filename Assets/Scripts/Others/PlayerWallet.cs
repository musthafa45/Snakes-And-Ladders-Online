using System;

public static class PlayerWallet
{
    public static event EventHandler<OnPlayerWalletModifiedArgs> OnPlayerWalletModified;
    public class OnPlayerWalletModifiedArgs : EventArgs
    {
        public float currentCashAmount;
    }
    private static float cashAmount = 2000;

    public static void AddCash(float amount)
    {
        cashAmount += amount;

        OnPlayerWalletModified?.Invoke(null,new OnPlayerWalletModifiedArgs
        {
            currentCashAmount = amount
        });
    }

    public static void DetuctCash(float amount)
    {
        cashAmount -= amount;

        OnPlayerWalletModified?.Invoke(null, new OnPlayerWalletModifiedArgs
        {
            currentCashAmount = amount
        });
    }

    public static float GetCurrentCashAmount()
    {
        return cashAmount;
    }
}
