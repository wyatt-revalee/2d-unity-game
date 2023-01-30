using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLCheck : MonoBehaviour
{
    public bool isWebGL;
    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            isWebGL = true;
        }
        else
        {
            isWebGL = false;
        }
    }
}