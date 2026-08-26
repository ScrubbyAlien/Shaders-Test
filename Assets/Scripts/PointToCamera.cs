using System;
using UnityEngine;

[ExecuteAlways]
public class PointToCamera : MonoBehaviour
{
    private Camera mainCamera;
    
    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        transform.rotation = mainCamera.transform.rotation;
    }
}
