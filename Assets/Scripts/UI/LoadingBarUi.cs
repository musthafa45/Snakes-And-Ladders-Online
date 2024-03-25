using UnityEngine;
using UnityEngine.UI;

public class LoadingBarUi : MonoBehaviour
{
    [SerializeField] private Image loadingBarImage;

    private void Awake()
    {
        loadingBarImage.fillAmount = 0;
    }
    private void Update()
    {
        loadingBarImage.fillAmount = Loader.GetLoadingProgress();
    }
}
