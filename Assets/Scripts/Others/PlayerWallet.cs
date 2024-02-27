using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerWallet
{
    private static int cashAmount = 200;

    public static void AddCash(int amount)
    {
        cashAmount += amount;
    }

    public static void DetuctCash(int amount)
    {
        cashAmount -= amount;
    }
}
