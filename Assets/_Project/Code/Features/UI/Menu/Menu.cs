using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    VisualElement mainMenu;
    VisualElement settingsMenu;
    [SerializeField] private Object playScene;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Button startButton = root.Q<Button>("Play");
        Button settingsButton = root.Q<Button>("Settings");
        Button exitButton = root.Q<Button>("Exit");
        Button backButton = root.Q<Button>("Back");

        mainMenu = root.Q<VisualElement>("MainMenu");
        settingsMenu = root.Q<VisualElement>("SettingsMenu");

        startButton.clicked += OnStartClicked;
        settingsButton.clicked += OnSettingsClicked;
        exitButton.clicked += OnExitClicked;
        backButton.clicked += OnBackClicked;

        settingsMenu.style.display = DisplayStyle.None;
    }

    void OnSettingsClicked()
    {
        mainMenu.style.display = DisplayStyle.None;
        settingsMenu.style.display = DisplayStyle.Flex;
    }

    void OnBackClicked()
    {
        settingsMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }

    void OnExitClicked()
    {
        Application.Quit();
    }

    void OnStartClicked()
    {
        SceneManager.LoadScene(playScene.name);
    }
}