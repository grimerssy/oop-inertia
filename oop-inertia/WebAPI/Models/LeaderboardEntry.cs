namespace WebAPI.Models;

public class LeaderboardEntry
{
    public string Name { get; }   
    public int Score { get; }

    public LeaderboardEntry(string name, int score)
    {
        Name = name;
        Score = score;
    }
}