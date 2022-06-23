using WebApplication1.Models;

namespace WebAPI.Models;

public class StartResponse
{
    public string[][] CellTypes {get;}
    public WebPlayer[] Players {get;}
    
    public StartResponse(string[][] cellTypes, WebPlayer[] players)
    {
        CellTypes = cellTypes;
        Players = players;
    }
}