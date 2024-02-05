using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetCodeUI : MonoBehaviour
{
    public static TestingNetCodeUI Instance { get; private set; }

    public event EventHandler OnPlayerClickedHostOrClientBtn;

    [SerializeField] private Button startHostBtn;
    [SerializeField] private Button startClientBtn;
    [SerializeField] private Transform menuUiTransform;


    private void Awake()
    {
        Instance = this;
        ShowUi();


        startHostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();

            HideUi();
            OnPlayerClickedHostOrClientBtn?.Invoke(this,EventArgs.Empty);

        });

        startClientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();

            HideUi();
            OnPlayerClickedHostOrClientBtn?.Invoke(this, EventArgs.Empty);
        });
    }

    private void OnDisable()
    {
        startHostBtn.onClick.RemoveAllListeners();
        startClientBtn.onClick.RemoveAllListeners();
    }

    private void HideUi()
    {
        menuUiTransform.gameObject.SetActive(false);
    }
    private void ShowUi()
    {
        menuUiTransform.gameObject.SetActive(true);
    }
}
