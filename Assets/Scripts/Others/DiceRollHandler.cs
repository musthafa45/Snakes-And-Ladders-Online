using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRollHandler : MonoBehaviour
{
    private readonly int diceRollPossibilities = 6; //Six Faces 
    private int previousSelectedNumber = 0;


    private void Start()
    {
        
    }

    private void EventManager_Instance_OnDiceRolled(object sender, EventManager.OnDiceRollButtonPerformedArgs e)
    {
        int randomNumber;
        do
        {
            randomNumber = GetRandomNumber();

        } while (previousSelectedNumber == randomNumber);

        previousSelectedNumber = randomNumber;
        EventManager.Instance.InvokeOnDiceRolled(randomNumber, e.diceRolledPlayerId);
    }

    //private void EventManager_Instance_OnDiceRolled(object sender, System.EventArgs e)
    //{
    //    int randomNumber; 
    //    do
    //    {
    //        randomNumber = GetRandomNumber();

    //    } while(previousSelectedNumber == randomNumber);

    //    previousSelectedNumber = randomNumber;
    //    EventManager.Instance.InvokeOnDiceRolled(randomNumber,e.diceRolledPlayerId);
    //}

    private int GetRandomNumber()
    {
        int randomNumber = UnityEngine.Random.Range(1, diceRollPossibilities + 1); // because Unity Random.Range Exclude 6
        Debug.Log("Selected Random Number " +  randomNumber);
        return randomNumber;
    }


}

