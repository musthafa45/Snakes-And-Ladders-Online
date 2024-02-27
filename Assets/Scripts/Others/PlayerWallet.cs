using System;

public static class PlayerWallet
{
    public static event EventHandler<OnPlayerWalletModifiedArgs> OnPlayerWalletModified;
    public class OnPlayerWalletModifiedArgs : EventArgs
    {
        public int currentCashAmount;
    }
    private static int cashAmount = 200;

    public static void AddCash(int amount)
    {
        cashAmount += amount;

        OnPlayerWalletModified?.Invoke(null,new OnPlayerWalletModifiedArgs
        {
            currentCashAmount = amount
        });
    }

    public static void DetuctCash(int amount)
    {
        cashAmount -= amount;

        OnPlayerWalletModified?.Invoke(null, new OnPlayerWalletModifiedArgs
        {
            currentCashAmount = amount
        });
    }

    public static int GetCurrentCashAmount()
    {
        return cashAmount;
    }
}
