namespace WebAPI.Models;

public class StartResponse
{
    public int PointsObjective { get; }
    public string[][] CellTypes { get; }
    public WebPlayer[] Players { get; }
    
    public StartResponse(int pointsObjective, string[][] cellTypes, WebPlayer[] players)
    {
        PointsObjective = pointsObjective;
        CellTypes = cellTypes;
        Players = players;
    }
}