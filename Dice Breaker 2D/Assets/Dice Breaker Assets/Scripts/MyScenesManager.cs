using UnityEngine;
using UnityEngine.SceneManagement;

public class MyScenesManager : MonoBehaviour
{
    public enum Scene { MainMenu, GameScene }
    public static MyScenesManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void LoadNewScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
    public void LoadGameScene()
    {
        SceneManager.LoadScene(Scene.GameScene.ToString());
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(Scene.MainMenu.ToString());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
