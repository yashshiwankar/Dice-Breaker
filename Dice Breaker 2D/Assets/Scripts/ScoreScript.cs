using UnityEngine;
using TMPro;
public class ScoreScript : MonoBehaviour
{
    public static ScoreScript Instance { get; private set; }
    
    [SerializeField] TextMeshProUGUI scoreText;
    int score;
    void Awake()
    {
        if(Instance == null)
            Instance = this;
        SetScore(0);
    }

    public void SetScore(int score)
    {
        this.score += score;
        scoreText.text = this.score.ToString();
    }
    public int GetScore()
    {
        return score;
    }
}
