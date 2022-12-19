using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class resizeCamera : MonoBehaviour {

    // Set this to the in-world distance between the left & right edges of your scene.
    // public float sceneWidth = 300;

    Camera _camera;
    void Start() {
        _camera = GetComponent<Camera>();
        _camera.pixelRect = new Rect(0, 0, 1600, 900);
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
    void Update() {
        // float unitsPerPixel = sceneWidth / Screen.width;

        // float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        // _camera.orthographicSize = desiredHalfHeight;
    }
}