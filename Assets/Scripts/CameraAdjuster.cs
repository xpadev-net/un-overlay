using System;
using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private Camera targetCamera;

    public void UpdateCameraPov()
    {
        var canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        var distance = Vector3.Distance(targetCamera.transform.position, canvas.transform.position);
        var fov = 2f * Mathf.Atan(canvasHeight / (2f * distance)) * Mathf.Rad2Deg;
        targetCamera.fieldOfView = fov;
    }
}