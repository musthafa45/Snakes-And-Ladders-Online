using UnityEngine;

public class PlayerProfileStatsHandlerUI : MonoBehaviour
{
    [SerializeField] private GameObject playersProfileStatsParent;
    private void Start()
    {
        Hide();

        GameManager.OnAnyGameManagerSpawned += (gameManager) =>
        {
           gameManager.GetComponent<GameManager>().OnStartMatchPerformed += (sender,e) => { Show(); }; //GameManager_LocalInstance_OnStartMatchPerformed;
        }; //GameManager_OnAnyGameManagerSpawned;
    }

    //private void GameManager_OnAnyGameManagerSpawned(object sender, System.EventArgs e)
    //{
    //    GameManager.LocalInstance.OnStartMatchPerformed += GameManager_LocalInstance_OnStartMatchPerformed;
    //}

    //private void OnDisable()
    //{
        //GameManager.LocalInstance.OnStartMatchPerformed -= GameManager_LocalInstance_OnStartMatchPerformed;
        //GameManager.OnAnyGameManagerSpawned -= GameManager_OnAnyGameManagerSpawned;
    //}

    //private void GameManager_LocalInstance_OnStartMatchPerformed(object sender, System.EventArgs e)
    //{
    //    Show();
    //}

    private void Show()
    {
        playersProfileStatsParent.SetActive(true);
    }

    private void Hide()
    {
        playersProfileStatsParent.SetActive(false);
    }
}
