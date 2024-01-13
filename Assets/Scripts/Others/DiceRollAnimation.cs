using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollAnimation : MonoBehaviour
{
    [SerializeField] private Image diceButtonImage;
    [SerializeField] private List<DiceData> diceDataList;

    public void RollDice(Action<short> onRollFinishedWithFaceValueOut)
    {
        StartCoroutine(RollAnim(onRollFinishedWithFaceValueOut));
    }

    private IEnumerator RollAnim(Action<short> onRollFinishedWithFaceValueOut)
    {
        float elapsedTime = 0f;
        float duration = 3f; // Set the duration of the roll illusion

        while (elapsedTime < duration)
        {
            // Randomly change the dice face sprite during the illusion
            diceButtonImage.sprite = GetRandomDiceData().diceFaceSprite;

            // Wait for a short time before the next change
            yield return new WaitForSeconds(0.1f);

            // Update the elapsed time
            elapsedTime += 0.1f;
        }

        // After the duration, set the final dice face sprite
        DiceData finalDiceData = GetRandomDiceData();
        diceButtonImage.sprite = finalDiceData.diceFaceSprite;

        onRollFinishedWithFaceValueOut?.Invoke(finalDiceData.diceFaceValue);
    }

    private DiceData GetRandomDiceData()
    {
        // Implement logic to get a random DiceData from the diceDataList
        // For example, you can use UnityEngine.Random.Range and diceDataList.Count
        int randomIndex = UnityEngine.Random.Range(0, diceDataList.Count);
        return diceDataList[randomIndex];
    }

}

[Serializable]
public class DiceData
{
    public Sprite diceFaceSprite;
    public short diceFaceValue;
}
