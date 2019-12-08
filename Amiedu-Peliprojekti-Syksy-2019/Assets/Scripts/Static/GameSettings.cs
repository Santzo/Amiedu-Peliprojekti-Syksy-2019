using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public static int lightingQuality = PlayerPrefs.GetInt("LightingQuality", 0);
    public static int resolutionX = PlayerPrefs.GetInt("ResolutionX", Screen.width);
    public static int resolutionY = PlayerPrefs.GetInt("ResolutionY", Screen.height);
    public static bool fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreen", 1));

    public static void ApplySettings()
    {
        QualitySettings.pixelLightCount = lightingQuality;
        Screen.SetResolution(resolutionX, resolutionY, fullScreen);
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
#if UNITY_EDITOR
        QualitySettings.pixelLightCount = 4;
#endif
    }
    public static void ApplyResolution(int x, int y)
    {
        PlayerPrefs.SetInt("ResolutionX", x);
        PlayerPrefs.SetInt("ResolutionY", y);
        resolutionX = x;
        resolutionY = y;
        Screen.SetResolution(x, y, fullScreen);
    }
    public static void ChangeFullScreen()
    {
        Debug.Log("toimii");
        Screen.fullScreen = !fullScreen;
        fullScreen = !fullScreen;
        PlayerPrefs.SetInt("FullScreen", Convert.ToInt32(fullScreen));
    }
    public static void ApplyLighting(int i)
    {
        if (i == 3) i = 4;
        QualitySettings.pixelLightCount = i;
        PlayerPrefs.SetInt("LightingQuality", i);
        lightingQuality = i;
    }

}
