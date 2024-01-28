using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    private void Awake()
    {
        menuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainMenu");
        });
    }

}

