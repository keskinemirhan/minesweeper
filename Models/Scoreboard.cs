using System.Collections.Generic;

namespace minesweeper.Models;

public struct PlayerScore
{
    public string Username { get; }
    public int Score { get; }
    public PlayerScore(string username, int score)
    {
        this.Username = username;
        this.Score = score;
    }

}

public class Scoreboard
{
    public List<PlayerScore> PlayerScoreList { get; }
    public void AddScore(string username, int score)
    {
        var playerScore = new PlayerScore(username, score);
        this.PlayerScoreList.Add(playerScore);
        this.PlayerScoreList.Sort((s1, s2) => s2.Score - s1.Score);
    }
    public Scoreboard()
    {
        this.PlayerScoreList = new List<PlayerScore>();
    }

}
