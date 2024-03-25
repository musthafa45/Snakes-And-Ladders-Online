using UnityEngine;

public class LoaderCallBack : MonoBehaviour
{
    private bool isFirstUpdate;

    private void Update()
    {
        if(!isFirstUpdate)
        {
            isFirstUpdate = true;
            Loader.LoaderCallBack();
        }
    }
}
