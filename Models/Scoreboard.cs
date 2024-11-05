using System.Collections.Generic;

namespace minesweeper.Models;

public struct PlayerScore
{
    public string username { get; }
    public int score { get; }
    public PlayerScore(string username, int score)
    {
        this.username = username;
        this.score = score;
    }

}

public class Scoreboard
{
    public List<PlayerScore> PlayerScoreList { get; }
    public void AddScore(string username, int score)
    {
        var playerScore = new PlayerScore(username, score);
        this.PlayerScoreList.Add(playerScore);
        this.PlayerScoreList.Sort((s1, s2) => s2.score - s1.score);
    }
    public Scoreboard()
    {
        this.PlayerScoreList = new List<PlayerScore>();
    }

}
