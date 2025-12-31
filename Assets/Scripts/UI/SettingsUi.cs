using UnityEngine;
using UnityEngine.UI;

public class SettingsUi : MonoBehaviour
{
    [SerializeField] private Transform settingsUi;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button resetProgressBtn, resetProgressNoBtn, resetProgressYesBtn;
    private void Awake() {
        settingsUi.gameObject.SetActive(false);

        resetProgressNoBtn.gameObject.SetActive(false);
        resetProgressYesBtn.gameObject.SetActive(false);

        exitButton.onClick.AddListener(() => {
            settingsUi.gameObject.SetActive(false);
            resetProgressNoBtn.gameObject.SetActive(false);
            resetProgressYesBtn.gameObject.SetActive(false);
        });

        resetProgressBtn.onClick.AddListener(() => {
            resetProgressNoBtn.gameObject.SetActive(true);
            resetProgressYesBtn.gameObject.SetActive(true);
        });

        resetProgressYesBtn.onClick.AddListener(() => {
            PlayerPrefs.DeleteAll();

            Application.Quit();

            resetProgressNoBtn.gameObject.SetActive(false);
            resetProgressYesBtn.gameObject.SetActive(false);
        });

        resetProgressNoBtn.onClick.AddListener(() => {
            resetProgressNoBtn.gameObject.SetActive(false);
            resetProgressYesBtn.gameObject.SetActive(false);
        });
    }
}
