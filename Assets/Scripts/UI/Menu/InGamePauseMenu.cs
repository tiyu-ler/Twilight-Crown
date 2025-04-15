using UnityEngine;
using UnityEngine.SceneManagement;

public class InGamePauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject MenuUi;
    public GameObject settingsMenuUI;
    public bool IsAbleToPause;
    private bool _isPaused = false;
    private GameManager _gameManager;
    void Start()
    {
        IsAbleToPause = true;
        pauseMenuUI.SetActive(false);
        MenuUi.SetActive(false);
        settingsMenuUI.SetActive(false);
        _gameManager = FindObjectOfType<GameManager>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsAbleToPause)
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenuUI.SetActive(true);
        MenuUi.SetActive(true);
        settingsMenuUI.SetActive(false);
        _isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        _isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        _gameManager.SaveGame(PlayerDataSave.Instance.saveID);
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettingsMenu()
    {
        MenuUi.SetActive(false);
        settingsMenuUI.SetActive(true);
    }
    public void CloseSettingsMenu()
    {
        MenuUi.SetActive(true);
        settingsMenuUI.SetActive(false);
    }
}
