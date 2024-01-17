using UnityEngine;

public class WaitigForOpponentUI : MonoBehaviour
{
    public static WaitigForOpponentUI Instance {  get; private set; }

    [SerializeField] private Transform waitingForOpponentUiParent;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        HideWaitingForOpponentUI();

        TestingNetCodeUI.Instance.OnPlayerClickedHostOrClientBtn += Testing_NetCodeUI_OnPlayerClickedHostOrClientBtn;

        GameManager.OnAnyGameManagerSpawned += (gameManager) =>
        {
            gameManager.GetComponent<GameManager>().OnStartMatchPerformed += (selectedPlayerId) => { 
                HideWaitingForOpponentUI(); 
            }; //GameManager_LocalInstance_OnStartMatchPerformed;
        };
    }

    private void Testing_NetCodeUI_OnPlayerClickedHostOrClientBtn(object sender, System.EventArgs e)
    {
        ShowWaitingForOpponentUI();
    }

    private void OnDisable()
    {
        TestingNetCodeUI.Instance.OnPlayerClickedHostOrClientBtn -= Testing_NetCodeUI_OnPlayerClickedHostOrClientBtn;
    }

    public void ShowWaitingForOpponentUI()
    {
        waitingForOpponentUiParent.gameObject.SetActive(true);
    }

    public void HideWaitingForOpponentUI()
    {
        waitingForOpponentUiParent.gameObject.SetActive(false);
    }
}
