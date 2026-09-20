using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance { get; private set; }

    public int score { get; private set; }

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int baseBlockDestScore = 100;
    [SerializeField] private int baseParryScore = 20;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        score = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BlockBehaviour.onDestroyedBlock.AddListener(AddBlockDestructionScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddParryScore(ParryData data)
    {
        if (!data.shouldContinueStreak) return;

        score += ParryStreakHandler.instance.parryStreak * baseParryScore;

        OnScoreUpdate();
    }

    public void AddBlockDestructionScore()
    {
        Debug.Log("SHOULD ADD POINTS");
        score += baseBlockDestScore * Mathf.RoundToInt(BallBehaviour.instance.speed);

        OnScoreUpdate();
    }

    private void OnScoreUpdate()
    {
        scoreText.text = "score : " + score.ToString("000000");
    }
}
