using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollAnimation : MonoBehaviour
{
    [SerializeField] private Image diceButtonImage;
    [SerializeField] private List<DiceData> diceDataList;

    public void RollDice(short selectedFaceValue, Action OnRollAnimFinished)
    {
        StartCoroutine(RollAnim(selectedFaceValue, OnRollAnimFinished));
    }

    private IEnumerator RollAnim(short selectedFaceValue, Action OnRollAnimFinished) {
        float elapsedTime = 0f;
        float duration = 1.2f;

        while (elapsedTime < duration) {
            diceButtonImage.sprite = GetRandomDiceData().diceFaceSprite;
            yield return new WaitForSeconds(0.08f);
            elapsedTime += 0.1f;
        }

        DiceData finalDiceData = diceDataList
            .FirstOrDefault(d => d.diceFaceValue == selectedFaceValue);

        if (finalDiceData != null) {
            diceButtonImage.sprite = finalDiceData.diceFaceSprite;

            OnRollAnimFinished?.Invoke();
        }
        else {
            Debug.LogError($"Dice face value {selectedFaceValue} not found!");
        }
    }

    public void PlayOpponentDiceRoll(short faceValue) {
        StartCoroutine(OpponentRollAnim(faceValue));
    }

    private IEnumerator OpponentRollAnim(short finalFaceValue) {
        float elapsedTime = 0f;
        float duration = 1.2f;

        while (elapsedTime < duration) {
            diceButtonImage.sprite = GetRandomDiceData().diceFaceSprite;
            yield return new WaitForSeconds(0.08f);
            elapsedTime += 0.1f;
        }

        DiceData finalDiceData = diceDataList
            .Find(d => d.diceFaceValue == finalFaceValue);

        diceButtonImage.sprite = finalDiceData.diceFaceSprite;
    }

    public List<DiceData> GetDiceDataList() => diceDataList;

    private DiceData GetRandomDiceData() {
        int randomIndex = UnityEngine.Random.Range(0, diceDataList.Count);
        return diceDataList[randomIndex];
    }
}