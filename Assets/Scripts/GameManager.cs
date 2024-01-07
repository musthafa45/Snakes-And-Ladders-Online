using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [SerializeField] private PlayMode playMode;

    private void Awake()
    {
        Instance = this;

        if (PlayerPrefs.GetInt("PlayMode") == 1)
        {
            playMode = PlayMode.MultiplayerLocal;
        }
        else if (PlayerPrefs.GetInt("PlayMode") == 2)
        {
            playMode = PlayMode.MultiplayerCom;
        }
        else if (PlayerPrefs.GetInt("PlayMode") == 3)
        {
            playMode = PlayMode.MultiplayerOnline;
        }
    }
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void SetPlayMode(PlayMode playMode) => this.playMode = playMode;
    public PlayMode GetPlayMode() => playMode;
}
public enum PlayMode
{
    MultiplayerLocal, MultiplayerCom, MultiplayerOnline,
}
