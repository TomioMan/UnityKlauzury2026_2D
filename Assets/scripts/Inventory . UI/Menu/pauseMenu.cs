using UnityEngine;
using UnityEngine.InputSystem;

public class pauseMenu : MonoBehaviour
{
    [Header("UI ELEMENTS")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject confirmationWindow;

    private CanvasGroup canvasGroup;
    private bool isPaused = false;

    void Awake()
    {
        if (pauseMenuCanvas != null)
        {
            canvasGroup = pauseMenuCanvas.GetComponent<CanvasGroup>();
        }

        ResumeGame();
        if (confirmationWindow != null) confirmationWindow.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            pauseMenuCanvas.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
        }

        CloseConfirmation();
    }

    public void OpenConfirmation()
    {
        if (confirmationWindow != null)
        {
            confirmationWindow.SetActive(true);
        }
    }

    public void CloseConfirmation()
    {
        if (confirmationWindow != null)
        {
            confirmationWindow.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}