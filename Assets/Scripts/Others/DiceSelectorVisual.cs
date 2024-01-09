using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DiceSelectorVisual : MonoBehaviour
{
    private Tweener tweener;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }

    [ContextMenu("DoVisual")]
    public void DoVisual()
    {
        image.enabled = true;

        transform.localScale = Vector3.one;
        tweener = transform.DOScale(1.07f,0.5f).SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);

    }

    [ContextMenu("StopVisual")]
    public void StopVisual()
    {
        tweener?.Kill();
        image.enabled = false;
    }
}
