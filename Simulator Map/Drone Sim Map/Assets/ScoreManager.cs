using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddPoint()
    {
        score++;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
    }
}