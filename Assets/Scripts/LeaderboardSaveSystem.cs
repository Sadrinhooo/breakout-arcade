using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LeaderboardSaveSystem
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");

    [System.Serializable]
    private class SaveData
    {
        public List<LeaderboardEntry> entries;
    }

    public static void Save(List<LeaderboardEntry> entries)
    {
        var data = new SaveData { entries = entries };
        string json = JsonUtility.ToJson(data, true);

        try
        {
            File.WriteAllText(SavePath, json);
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to save leaderboard: {e.Message}");
        }
    }

    public static List<LeaderboardEntry> Load()
    {
        if (!File.Exists(SavePath))
        {
            return new List<LeaderboardEntry>();
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data?.entries ?? new List<LeaderboardEntry>();
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to load leaderboard: {e.Message}");
            return new List<LeaderboardEntry>();
        }
    }
}
