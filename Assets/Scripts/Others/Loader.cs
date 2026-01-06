using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public class DummyClass : MonoBehaviour { }
    public enum Scene { MainMenu, Loading , QuickLobby , SelectLobby ,Game, PlayerVsComputerGame, PassAndPlayGame }

    private static Scene toLoadScene;
    private static AsyncOperation loadingAsyncOperation = null;

    public static void LoadScene(Scene scene)
    {
        toLoadScene = scene;
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallBack()
    {
        GameObject loadingGameObj = new("Loading GameObj");
        loadingGameObj.AddComponent<DummyClass>().StartCoroutine(LoadSceneAsync(toLoadScene));
    }

    private static IEnumerator LoadSceneAsync(Scene toLoadScene)
    {
        yield return null;
        loadingAsyncOperation = SceneManager.LoadSceneAsync(toLoadScene.ToString());

        while(!loadingAsyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if(loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }
}
