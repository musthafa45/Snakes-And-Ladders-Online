using UnityEngine;
using UnityEngine.UI;

public class SearchingForOpponentUI : MonoBehaviour
{
    [SerializeField] private Image loadingImage;
    [SerializeField] private float rotationSpeed;

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
