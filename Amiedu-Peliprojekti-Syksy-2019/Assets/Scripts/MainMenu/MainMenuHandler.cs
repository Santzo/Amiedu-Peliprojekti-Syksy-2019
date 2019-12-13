using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour, IMainMenuHandler
{
    private TextMeshProUGUI[] menuOptions;
    private Resolution[] resolutions;
    private GraphicRaycaster gr;
    [HideInInspector]
    public MenuState menuState;
    private Dictionary<string, KeyCode[]> keys = new Dictionary<string, KeyCode[]>();
    private FieldInfo[] keyFields;
    public GameObject confirmKey;
    private GameObject currentConfirmKey;
    public Transform zombieEye;
    public Transform zombieRightEye;
    private Vector2 eyeBasePos;
    private Camera mainCamera;
    private GameObject currentConfirmKeyWarning;
    private Slider slider;
    private Animator[] animators;
    private CustomSlider masterSlider, musicSlider, soundSlider;
    const int spacing = 60;
    const int textSpacing = 60;
    private float oriFontSize;
    int currentKeyBeingSet;
    public bool newKeyBeingSet { get; set; }

    public enum MenuState
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
        mainCamera = Camera.main;
        gr = GetComponentInParent<GraphicRaycaster>();
        keyFields = typeof(KeyboardConfig).GetFields();
        slider = transform.parent.Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(SliderListener);
        animators = GetComponentsInChildren<Animator>();
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
        resolutions = FetchResolutions();
        eyeBasePos = new Vector2(zombieEye.position.x, zombieEye.position.y);
        InitAudioSliders();
        SwitchMenuState(MenuState.Main, 4);
    }


    private void InitAudioSliders()
    {
        masterSlider = transform.parent.Find("MasterSlider").GetComponent<CustomSlider>();
        musicSlider = transform.parent.Find("MusicSlider").GetComponent<CustomSlider>();
        soundSlider = transform.parent.Find("SoundSlider").GetComponent<CustomSlider>();
        masterSlider.gameObject.SetActive(false);
        musicSlider.gameObject.SetActive(false);
        soundSlider.gameObject.SetActive(false);

    }
    private void SetAudioSliders()
    {
        masterSlider.gameObject.SetActive(true);
        musicSlider.gameObject.SetActive(true);
        soundSlider.gameObject.SetActive(true);
        masterSlider.value = Audio.ad.MasterVolume;
        musicSlider.value = Audio.ad.MusicVolume;
        soundSlider.value = Audio.ad.SoundVolume;
        masterSlider.onValueChanged.AddListener(val => Audio.ad.MasterVolume = val);
        musicSlider.onValueChanged.AddListener(val => Audio.ad.MusicVolume = val);
        soundSlider.onValueChanged.AddListener(val => Audio.ad.SoundVolume = val);
    }
    private void DisableAudioSliders()
    {
        masterSlider.gameObject.SetActive(false);
        musicSlider.gameObject.SetActive(false);
        soundSlider.gameObject.SetActive(false);
        masterSlider.value = Audio.ad.MasterVolume;
        musicSlider.value = Audio.ad.MusicVolume;
        soundSlider.value = Audio.ad.SoundVolume;
        masterSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        soundSlider.onValueChanged.RemoveAllListeners();
        Audio.ad.SaveSettings();
    }
    private void SliderListener(float amount)
    {
        float y = amount;
        transform.localPosition = new Vector2(transform.localPosition.x, y);
    }

    void Update()
    {
        if (newKeyBeingSet)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        string keyName = keyFields[currentKeyBeingSet].Name;
                        foreach (var k in keys)
                        {
                            if (k.Value[0] == key && k.Key != keyName)
                            {
                                Audio.PlaySound("Error", 0.9f, 0.7f);
                                currentConfirmKeyWarning.SetActive(false);
                                currentConfirmKeyWarning.SetActive(true);
                                return;
                            }
                        }
                        Destroy(currentConfirmKey);
                        keys[keyName][0] = key;
                        KeyboardConfig.SaveKey(keyName, key);
                        SwitchMenuState(MenuState.ControlSettings, keys.Count + 1);
                        StartCoroutine(KeyChanged());
                        return;
                    }
                }
            }
        }
        if (menuState == MenuState.Main) return;
        var a = Input.GetAxis("Mouse ScrollWheel");
        if (a != 0 && slider.gameObject.activeSelf)
        {
            slider.value += -Input.mouseScrollDelta.y * slider.maxValue / 15f;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) GoBack();
    }
    IEnumerator KeyChanged()
    {
        yield return new WaitForSeconds(0.1f);
        newKeyBeingSet = false;
    }
    void LateUpdate()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 setPos = (mousePos - eyeBasePos).normalized;
        zombieEye.position = (Vector2)zombieEye.position + setPos * 0.11f;
        zombieRightEye.position = (Vector2)zombieRightEye.position + setPos * 0.07f;
    }

    private void GoBack()
    {
        Audio.PlaySound("Click", 1f, 0.8f);
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
        if (newKeyBeingSet) return;
        if (menuState == MenuState.Main)
        {
            switch (trans.name)
            {
                case "Option1":
                    Audio.VolumeFade("MainMenuMusic", 1f, 0f, 1.5f, false, true);
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
                    SwitchMenuState(MenuState.AudioSettings, 5);
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
            else
            {
                currentKeyBeingSet = index;
                newKeyBeingSet = true;
                currentConfirmKey = Instantiate(confirmKey, transform.parent.parent);
                currentConfirmKey.GetComponentInChildren<TextMeshProUGUI>().text = $"select a new key for {TextColor.Yellow}{keyFields[index].Name}";
                currentConfirmKeyWarning = currentConfirmKey.transform.Find("Warning").gameObject;
            }
        }
        else if (menuState == MenuState.AudioSettings)
        {
            int index = trans.GetSiblingIndex();
            if (index == 4) SwitchMenuState(MenuState.Settings, 4);
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
            var a = accepted.Any(ac => ac.width == res.width);
            if (rightResolution && !a) accepted.Add(res);
        }
        accepted.Reverse();
        return accepted.ToArray();
    }

    public void OnEnter(Transform trans)
    {

    }

    public void OnExit(Transform trans)
    {

    }

    void SwitchMenuState(MenuState newMenu, int options, int? index = null)
    {
        if (menuState == MenuState.AudioSettings) DisableAudioSliders();
        slider.gameObject.SetActive(options > 5);
        slider.maxValue = options > 5 ? (options - 5) * spacing : 1;
        for (int i = 0; i < menuOptions.Length; i++)
        {
            animators[i].SetBool("Hover", false);
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
        else if (newMenu == MenuState.Main)
        {
            menuOptions[0].text = "Start Game";
            menuOptions[1].text = "Load Game";
            menuOptions[2].text = "Settings";
            menuOptions[3].text = "Quit Game";
        }
        else if (newMenu == MenuState.VideoSettings)
        {
            menuOptions[0].text = $"Resolution {TextColor.Yellow}{GameSettings.resolutionX}{TextColor.White}x{TextColor.Yellow}{GameSettings.resolutionY}";
            menuOptions[1].text = $"Lighting Quality {TextColor.Yellow}{Lighting(QualitySettings.pixelLightCount)}";
            menuOptions[2].text = $"Fullscreen mode {TextColor.Yellow}" + (GameSettings.fullScreen ? "On" : "Off");
            menuOptions[3].text = "Back to Settings";
        }
        else if (newMenu == MenuState.LightingQuality)
        {
            menuOptions[0].text = $"{TextColor.Yellow}Low";
            menuOptions[1].text = $"{TextColor.Yellow}Medium";
            menuOptions[2].text = $"{TextColor.Yellow}High";
            menuOptions[3].text = $"{TextColor.Yellow}Highest";
            menuOptions[4].text = "Back to Video Settings";
        }
        else if (newMenu == MenuState.Resolutions)
        {
            for (int i = 0; i < resolutions.Length; i++)
            {
                menuOptions[i].text = $"{TextColor.Yellow}{resolutions[i].width}{TextColor.White}x{TextColor.Yellow}{resolutions[i].height}";
            }
            menuOptions[resolutions.Length].text = "Back to Video Settings";
        }
        else if (newMenu == MenuState.ControlSettings)
        {
            int i = 0;
            foreach (var key in keys)
            {
                menuOptions[i].text = $"{key.Key} {TextColor.Yellow}{KeyboardConfig.ReturnKeyName(key.Value[0].ToString())}";
                i++;
            }
            menuOptions[keys.Count].text = "Back to Settings";
        }
        else if (newMenu == MenuState.AudioSettings)
        {
            SetAudioSliders();
            menuOptions[0].text = "Master Volume";
            menuOptions[1].text = "Music Volume";
            menuOptions[2].text = "Sound Effects";
            menuOptions[3].gameObject.SetActive(false);
            menuOptions[4].text = "Back to Settings";
        }
        CheckRayCast();
    }

    private void CheckRayCast()
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);
        if (results.Count > 0)
        {
            int a = results[0].gameObject.transform.GetSiblingIndex();
            if (menuState == MenuState.AudioSettings && a < 3) return;
            animators[a].SetBool("Hover", true);
        }
    }

    public string Lighting(int index)
    {
        if (index == 0) return "Low";
        if (index == 1) return "Medium";
        if (index == 2) return "High";
        if (index == 4) return "Highest";
        return "Blaa";

    }
}

