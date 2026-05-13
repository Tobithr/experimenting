using UnityEngine;
using UnityEngine.InputSystem; // Wichtig für das neue Input System

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI; // Dein "PauseMenu" Empty Object

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

    public void Settings()
    {
        
    }
}