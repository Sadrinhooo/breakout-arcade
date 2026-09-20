using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct LeaderboardEntry
{
    public string entryName;
    public int score;
}
public class Leaderboard : MonoBehaviour
{
    [SerializeField] private NameCreator nameCreator;


    [Header("Leaderboard")]
    [SerializeField] private GameObject leaderboardOverlay;
    [SerializeField] private TMP_Text TMP_score;
    [SerializeField] private TMP_Text TMP_streak;
    [SerializeField] private TMP_Text TMP_time;
    [SerializeField] private TMP_Text TMP_leaderboardContent;


    private int leaderboardLength = 10;

    private bool leaderBoardActive = false;
    public static List<LeaderboardEntry> entries { get; private set; }



    private void Awake()
    {
        leaderBoardActive = false;
        if (entries == null)
        {
            entries = LeaderboardSaveSystem.Load();
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leaderboardOverlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowLeaderboard()
    {
        leaderBoardActive = true;
        leaderboardOverlay.SetActive(true);
        TMP_score.text = "score: " + ScoreManager.instance.score;
        TMP_streak.text = "best streak: " + ParryStreakHandler.instance.highestStreak;
        TMP_time.text = "time: " + (int)Timer.gameDuration;
        DisplayLeaderboard();
        InputSystem.onAnyButtonPress.CallOnce(RestartGame);
    }

    public void TrySubmitScore()
    {
        int score = ScoreManager.instance.score;
        bool skipNaming = true;

        if (entries.Count < leaderboardLength)
        {
            skipNaming = false; 
            nameCreator.BeginNameEntry(finishedName =>
            {
                entries.Add(new LeaderboardEntry { entryName = finishedName, score = score });
                entries = entries.OrderByDescending(e => e.score).ToList();
                LeaderboardSaveSystem.Save(entries);
                ShowLeaderboard();
            });

        } 
        else
        {
            entries = entries.OrderByDescending(e => e.score).ToList();

            foreach (var i in entries)
            {
                if (score > i.score)
                {
                    skipNaming = false;
                    entries.RemoveAt(entries.Count - 1);
                    nameCreator.BeginNameEntry(finishedName =>
                    {
                        entries.Add(new LeaderboardEntry { entryName = finishedName, score = score });
                        entries = entries.OrderByDescending(e => e.score).ToList();
                        LeaderboardSaveSystem.Save(entries);
                        ShowLeaderboard();
                    });
                    break;
                }
            }
        }

        if (skipNaming) ShowLeaderboard();  

    }

    public void DisplayLeaderboard()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<mspace=0.6em>");

        for (int i = 0; i < entries.Count; i++)
        {
            string rank = (i + 1).ToString().PadLeft(2);
            string name = entries[i].entryName.PadRight(6);
            string score = entries[i].score.ToString().PadLeft(6);

            sb.AppendLine($"{rank}. {name}-{score}");
        }

        sb.AppendLine("</mspace>");
        TMP_leaderboardContent.text = sb.ToString();
    }

    private void RestartGame(InputControl control)
    {
        SceneManager.LoadScene(0);
    }
}
