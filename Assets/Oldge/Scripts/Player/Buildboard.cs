/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildboard : MonoBehaviour
{
    Camera cam;

    void Update()
    {
        if (cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }

        if (cam == null)
        {
            return;
        }

        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);
    }
}*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildboard : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        // Initialize the camera in Start to ensure it's set before Update is called
        cam = Camera.main;
    }

    void Update()
    {
        // Re-check if cam is null (in case camera is not set in Start)
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            return;
        }

        // Use the camera's up vector to maintain the up direction
        transform.LookAt(cam.transform, Vector3.up);
        transform.Rotate(Vector3.up * 180);
    }
}
