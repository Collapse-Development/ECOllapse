using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem.Composites;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    //main
    VisualElement mainMenu;
    VisualElement settingsMenu;
    //start
    VisualElement createSave;
    VisualElement createWorld;
    //setting
    VisualElement soundSetting;
    VisualElement gameSetting;
    VisualElement inputSetting;
    VisualElement graphicSetting;
    
    [SerializeField] private Object playScene;
    List<VisualElement> settingElements;
    Slider volumeSlider;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        //main
        Button startButton = root.Q<Button>("Play");
        Button settingsButton = root.Q<Button>("Settings");
        Button exitButton = root.Q<Button>("Exit");
        //start
        Button save1Button = root.Q<Button>("Save1");
        Button save2Button = root.Q<Button>("Save2");
        Button save3Button = root.Q<Button>("Save3");
        Button backSaveButton = root.Q<Button>("BackSave");
        Button createButton = root.Q<Button>("CreateButton");
        Button backCreateButton = root.Q<Button>("BackCreate");
        //setting
        Button backButton = root.Q<Button>("Back");
        Button soundButton = root.Q<Button>("Sound");
        Button gameButton = root.Q<Button>("Game");
        Button inputButton = root.Q<Button>("Input");
        Button graphicButton = root.Q<Button>("Graphic");
        //sound
        Button soundReturn = root.Q<Button>("SoundReturn");


        mainMenu = root.Q<VisualElement>("MainMenu");
        createSave = root.Q<VisualElement>("CreateSave");
        createWorld = root.Q<VisualElement>("CreateWorld");
        settingsMenu = root.Q<VisualElement>("SettingsMenu");
        gameSetting = root.Q<VisualElement>("GameSetting");
        gameSetting = root.Q<VisualElement>("GameSetting");
        soundSetting = root.Q<VisualElement>("SoundSetting");
        inputSetting = root.Q<VisualElement>("InputSetting");
        graphicSetting = root.Q<VisualElement>("GraphicSetting");

        volumeSlider = root.Q<Slider>("SoundControl");

        settingElements = new List<VisualElement>()
        {
            gameSetting,
            inputSetting,
            graphicSetting,
            soundSetting
        };

        //main
        startButton.clicked += OnStartClicked;
        settingsButton.clicked += OnSettingsClicked;
        exitButton.clicked += OnExitClicked;
        //start
        save1Button.clicked += OnEmptySave1Clicked;
        save2Button.clicked += OnEmptySave2Clicked;
        save3Button.clicked += OnEmptySave3Clicked;
        backSaveButton.clicked += OnBackSaveClicked;
        createButton.clicked += OnCreateWorldClicked;
        backCreateButton.clicked += OnBackCreateClicked; 
        //setting
        soundButton.clicked += OnSoundClicked;
        backButton.clicked += OnBackClicked;
        gameButton.clicked += OnGameClicked;
        graphicButton.clicked += OnGraphicClicked;
        inputButton.clicked += OnInputClicked;
        //sound
        soundReturn.clicked += OnSoundReturnClicked;

        settingsMenu.style.display = DisplayStyle.None;

    }


    void OnSettingsClicked()
    {
        mainMenu.style.display = DisplayStyle.None;
        settingsMenu.style.display = DisplayStyle.Flex;
    }
    void OnSoundClicked()
    {
        HideSettings();
        soundSetting.style.display = DisplayStyle.Flex;
    }
    void OnSoundReturnClicked()
    {
        volumeSlider.value = 50;
    }
    void OnGameClicked()
    {
        HideSettings();
        gameSetting.style.display = DisplayStyle.Flex;
    }
    void OnInputClicked()
    {
        HideSettings();
        inputSetting.style.display = DisplayStyle.Flex;
    }
    void OnGraphicClicked()
    {
        HideSettings();
        graphicSetting.style.display = DisplayStyle.Flex;
    }


    void OnBackClicked()
    {
        HideSettings();
        settingsMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }
    void HideSettings()
    {
        foreach (var element in settingElements)
        {
            element.style.display = DisplayStyle.None;
        }
    }
    void OnExitClicked()
    {
        Application.Quit();
    }

    void OnStartClicked()
    {
        mainMenu.style.display = DisplayStyle.None;
        createSave.style.display = DisplayStyle.Flex;
    }
    void OnEmptySave1Clicked()
    {
        createSave.style.display = DisplayStyle.None;
        createWorld.style.display = DisplayStyle.Flex;
    }
    void OnEmptySave2Clicked()
    {
        createSave.style.display = DisplayStyle.None;
        createWorld.style.display = DisplayStyle.Flex;
    }
    void OnEmptySave3Clicked()
    {
        createSave.style.display = DisplayStyle.None;
        createWorld.style.display = DisplayStyle.Flex;
    }
    void OnBackSaveClicked()
    {
        createSave.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }
    void OnCreateWorldClicked()
    {
        SceneManager.LoadScene(playScene.name);
    }
    void OnBackCreateClicked()
    {
        createWorld.style.display = DisplayStyle.None;
        createSave.style.display = DisplayStyle.Flex;
    }
}