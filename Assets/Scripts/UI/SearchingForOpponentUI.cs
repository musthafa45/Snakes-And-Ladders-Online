using System;
using UnityEngine;
using UnityEngine.UI;

public class SearchingForOpponentUI : MonoBehaviour
{
    [SerializeField] private Image loadingImage;
    [SerializeField] private float rotationSpeed;


    private void Start()
    {
        if(SelectLobbyUi.Instance != null)
        {
            SelectLobbyUi.Instance.OnPlayButtonClicked += SelectLobbyUi_OnPlayButtonClicked;


            Hide();
        }
        
    }

    private void SelectLobbyUi_OnPlayButtonClicked(object sender, SelectLobbyUi.OnPlayButtonClickedArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // Get the current rotation of the image
        Vector3 currentRotation = loadingImage.transform.localEulerAngles;

        // Add the rotation speed to the current rotation on the Z-axis
        currentRotation.z -= rotationSpeed * Time.deltaTime;

        // Apply the new rotation
        loadingImage.transform.localEulerAngles = currentRotation;
    }


}
