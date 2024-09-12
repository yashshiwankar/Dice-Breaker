using UnityEngine;
using UnityEngine.UI;
public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private Image[] heart;
    [SerializeField]
    private int maxHealth = 3;
    private int currentHealth;
    public static HealthManager Instance { get; private set; }

    public delegate void GameOver();
    public event GameOver OnGameOver;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        currentHealth = maxHealth;
    }

    void Update()
    {
        foreach(var img in heart)
        {
            img.color = Color.grey;
        }
        for(int i = 0; i < currentHealth; i++)
        {
            heart[i].color = Color.red;
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SubtractCurrentHealth(int amt)
    {
        if(currentHealth >= amt)
        {
            currentHealth -= amt;
        }
        if(currentHealth <= 0)
        {
            OnGameOver?.Invoke();
        }
    }
}
