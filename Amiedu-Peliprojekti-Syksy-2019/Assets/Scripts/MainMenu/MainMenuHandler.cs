using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour, IMainMenuHandler
{
    private TextMeshProUGUI[] menuOptions;
    private Resolution[] resolutions;
    private MenuState menuState;
    private Dictionary<string, KeyCode[]> keys = new Dictionary<string, KeyCode[]>();
    private FieldInfo[] keyFields;
    private Slider slider;
    const int spacing = 58;
    const int textSpacing = 60;
    private float oriFontSize;

    private enum MenuState
    {
        Main,
        Settings,
        AudioSettings,
        VideoSettings,
        ControlSettings,
        LightingQuality,
        Resolutions
    }

    private void Awake()
    {
        keyFields = typeof(KeyboardConfig).GetFields();
        slider = transform.parent.Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(SliderListener);
        slider.gameObject.SetActive(false);
        foreach (var key in keyFields)
        {
            KeyCode[] temp = key.GetValue(null) as KeyCode[];
            if (temp != null) keys.Add(key.Name, temp);
        }
        menuOptions = GetComponentsInChildren<TextMeshProUGUI>();
        Vector2 oriPos = menuOptions[0].transform.localPosition;
        for (int i = 0; i < menuOptions.Length; i++)
        {
            menuOptions[i].transform.localPosition = new Vector2(oriPos.x, oriPos.y - i * textSpacing);
        }
        oriFontSize = menuOptions[0].fontSize;
        SwitchMenuState(MenuState.Main, 4);
        resolutions = FetchResolutions();
    }

    private void SliderListener(float amount)
    {
        float y = amount;
        transform.localPosition = new Vector2(transform.localPosition.x, y);
    }

    void Update()
    {
        if (menuState == MenuState.Main) return;
        var a = Input.GetAxis("Mouse ScrollWheel");
        if (a != 0 && slider.gameObject.activeSelf)
        {
            slider.value += -Input.mouseScrollDelta.y * slider.maxValue / 15f;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) GoBack();
    }

    private void GoBack()
    {
        switch (menuState)
        {
            case MenuState.Resolutions:
            case MenuState.LightingQuality:
                SwitchMenuState(MenuState.VideoSettings, 4);
                break;
            case MenuState.VideoSettings:
            case MenuState.AudioSettings:
            case MenuState.ControlSettings:
                SwitchMenuState(MenuState.Settings, 4);
                break;
            case MenuState.Settings:
                SwitchMenuState(MenuState.Main, 4);
                break;


        }
    }

    public void OnClick(Transform trans)
    {
        if (menuState == MenuState.Main)
        {
            switch (trans.name)
            {
                case "Option1":
                    SceneManager.LoadScene("Game");
                    break;

                case "Option2":
                    Debug.Log("Load Game");
                    break;

                case "Option3":
                    SwitchMenuState(MenuState.Settings, 4);
                    break;

                case "Option4":
                    Application.Quit();
                    break;
            }
        }
        else if (menuState == MenuState.Settings)
        {
            switch (trans.name)
            {
                case "Option1":
                    SwitchMenuState(MenuState.ControlSettings, keys.Count + 1);
                    break;

                case "Option2":
                    SwitchMenuState(MenuState.VideoSettings, 4);
                    break;

                case "Option3":

                    break;

                case "Option4":
                    SwitchMenuState(MenuState.Main, 4);
                    break;
            }
        }
        else if (menuState == MenuState.VideoSettings)
        {
            switch (trans.name)
            {
                case "Option1":
                    SwitchMenuState(MenuState.Resolutions, resolutions.Length + 1);
                    break;
                case "Option2":
                    SwitchMenuState(MenuState.LightingQuality, 5);
                    break;

                case "Option3":
                    GameSettings.ChangeFullScreen();
                    SwitchMenuState(MenuState.VideoSettings, 4);
                    break;

                case "Option4":
                    SwitchMenuState(MenuState.Settings, 4);
                    break;
            }
        }
        else if (menuState == MenuState.Resolutions)
        {
            int index = trans.GetSiblingIndex();
            if (index == resolutions.Length) SwitchMenuState(MenuState.VideoSettings, 4);
            else
            {
                GameSettings.ApplyResolution(resolutions[index].width, resolutions[index].height);
                SwitchMenuState(MenuState.VideoSettings, 4);
            }
        }
        else if (menuState == MenuState.LightingQuality)
        {
            int index = trans.GetSiblingIndex();
            if (index == 4) SwitchMenuState(MenuState.VideoSettings, 4);
            else
            {
                GameSettings.ApplyLighting(index);
                SwitchMenuState(MenuState.VideoSettings, 4);
            }
        }
        else if (menuState == MenuState.ControlSettings)
        {
            int index = trans.GetSiblingIndex();
            if (index == keys.Count) SwitchMenuState(MenuState.Settings, 4);
        }
    }

    private Resolution[] FetchResolutions()
    {
        var resolutions = Screen.resolutions;
        List<Resolution> accepted = new List<Resolution>();
        foreach (var res in resolutions)
        {
            float multiplier = (float)res.width / 16f;
            bool rightResolution = Mathf.RoundToInt(multiplier * 9f) == res.height;
            if (rightResolution) accepted.Add(res);

        }
        return accepted.ToArray();
    }

    public void OnEnter(Transform trans)
    {

    }

    public void OnExit(Transform trans)
    {

    }
    void SwitchMenuState(MenuState newMenu, int options, object obj = null)
    {
        slider.gameObject.SetActive(options > 5);
        slider.maxValue = options > 5 ? (options - 5) * spacing : 1;
        for (int i = 0; i < menuOptions.Length; i++)
        {
            menuOptions[i].gameObject.SetActive(i < options);
            menuOptions[i].fontSize = oriFontSize;
        }

        menuState = newMenu;
        if (newMenu == MenuState.Settings)
        {
            menuOptions[0].text = "Controls";
            menuOptions[1].text = "Video Settings";
            menuOptions[2].text = "Audio Settings";
            menuOptions[3].text = "Back to Main Menu";
        }
        if (newMenu == MenuState.Main)
        {
            menuOptions[0].text = "Start Game";
            menuOptions[1].text = "Load Game";
            menuOptions[2].text = "Settings";
            menuOptions[3].text = "Quit Game";
        }
        if (newMenu == MenuState.VideoSettings)
        {
            menuOptions[0].text = $"Resolution {TextColor.Return("yellow")}{GameSettings.resolutionX}{TextColor.Return()}x{TextColor.Return("yellow")}{GameSettings.resolutionY}";
            menuOptions[1].text = $"Lighting Quality {TextColor.Return("yellow")}{Lighting(QualitySettings.pixelLightCount)}";
            menuOptions[2].text = $"Fullscreen mode {TextColor.Return("yellow")}" + (GameSettings.fullScreen ? "On" : "Off");
            menuOptions[3].text = "Back to Settings";
        }
        if (newMenu == MenuState.LightingQuality)
        {
            menuOptions[0].text = $"{TextColor.Return("yellow")}Low";
            menuOptions[1].text = $"{TextColor.Return("yellow")}Medium";
            menuOptions[2].text = $"{TextColor.Return("yellow")}High";
            menuOptions[3].text = $"{TextColor.Return("yellow")}Highest";
            menuOptions[4].text = "Back to Video Settings";
        }
        if (newMenu == MenuState.Resolutions)
        {
            for (int i = 0; i < resolutions.Length; i++)
            {
                menuOptions[i].text = $"{TextColor.Return("yellow")}{resolutions[i].width}{TextColor.Return()}x{TextColor.Return("yellow")}{resolutions[i].height}";
            }
            menuOptions[resolutions.Length].text = "Back to Video Settings";
        }
        if (newMenu == MenuState.ControlSettings)
        {
            int i = 0;
            foreach (var key in keys)
            {
                menuOptions[i].text = $"{key.Key} {TextColor.Return("yellow")}{KeyboardConfig.ReturnKeyName(key.Value[0].ToString())}";
                i++;
            }
            menuOptions[keys.Count].text = "Back to Settings";
        }
    }
    public string Lighting(int index)
    {
        if (index == 0) return "Low";
        if (index == 1) return "Medium";
        if (index == 2) return "High";
        if (index == 3) return "Highest";
        if (index == 4) return "Highest";
        return "Blaa";

    }
}

