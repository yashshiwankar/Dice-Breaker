using UnityEngine;
public class UIManager : MonoBehaviour
{
    public static UIManager instance {get; private set;}
    private static MyScenesManager myScenesManager;

    private void Awake()
    {
        if(instance == null)
        instance = this;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        MyScenesManager.instance.LoadGameScene();
    }

    public void QuitGame()
    {
        MyScenesManager.instance.QuitGame();
    }
}
