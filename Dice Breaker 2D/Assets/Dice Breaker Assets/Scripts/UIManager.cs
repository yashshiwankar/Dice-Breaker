using UnityEngine;
public class UIManager : MonoBehaviour
{
    public static UIManager instance {get; private set;}
    private static MyScenesManager myScenesManager;
    [SerializeField] GameObject pauseScreenUI;

    private void Awake()
    {
        if(instance == null)
        instance = this;
    }

    public void TogglePause()
    {
        if(Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            pauseScreenUI.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            pauseScreenUI.SetActive(true);
        }

        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
    }

    public void Resume()
    {
        pauseScreenUI.SetActive(false);
        Time.timeScale = 1f;
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
        MyScenesManager.instance.LoadGameScene();
    }
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
        MyScenesManager.instance.LoadMainMenu();
    }

    public void QuitGame()
    {
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
        MyScenesManager.instance.QuitGame();
    }
}
