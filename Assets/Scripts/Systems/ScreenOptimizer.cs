using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ScreenOptimizer : MonoBehaviour
{
    private void Start()
    {
        //get pixel perfect camera

        var camera = GetComponent<PixelPerfectCamera>();

        if (camera)
        {
            Debug.Log("asdfasdf");
            //set reference resolution height to aspect ratio of screen based on the width
            camera.refResolutionY = (int) (camera.refResolutionX * ((float) Screen.height / Screen.width));

        }
    }
}
