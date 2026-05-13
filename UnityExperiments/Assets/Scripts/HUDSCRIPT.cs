using UnityEngine;
using UnityEngine.InputSystem; // Wichtig für das neue Input System
using UnityEngine.UI; // Für Slider
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI; // Dein "PauseMenu" Empty Object
    [SerializeField] private GameObject settingsMenuUI;

    [Header("Settings Controls")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private PlayerMovementNew playerScript;

    private InputSystem_Actions controls;
    private bool isPaused = false;

    private void Awake()
    {
        controls = new InputSystem_Actions();

        // Hier abonnieren wir deine neue Action "Pause"
        // .performed bedeutet: Die Taste wurde gerade gedrückt
        controls.Player.Pause.performed += ctx => TogglePause();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Menü ausblenden
        //Time.timeScale = 1f;          // Zeit läuft normal weiter
        isPaused = false;

        // Maus wieder für das Gameplay sperren
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);  // Menü einblenden
        //Time.timeScale = 0f;          // Alles im Spiel einfrieren
        isPaused = true;

        // Maus befreien, um Buttons zu klicken
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Diese Funktion verknüpfst du mit dem "Quit" Button im Inspector
    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet...");
        Application.Quit();
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
        
        // Hier könntest du gespeicherte Werte in die Slider laden
        // volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    // Wird vom Volume-Slider aufgerufen
    public void SetVolume(float value)
    {
        // Einfachste Methode: Master Volume der ganzen App
        AudioListener.volume = value;
        Debug.Log("Volume: " + value);
    }

    // Wird vom Sensitivity-Slider aufgerufen
    public void SetSensitivity(float value)
    {
        // Hier setzen wir den Wert im Player-Skript
        // Du müsstest in deinem Player-Skript eine Variable 'mouseSensitivity' haben
        // playerScript.mouseSensitivity = value;
        Debug.Log("Sensitivität: " + value);
    }
}