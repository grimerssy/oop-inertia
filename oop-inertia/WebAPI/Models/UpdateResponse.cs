using WebApplication1.Models;

namespace WebAPI.Models;

public class UpdateResponse
{
    public string PrevCoordinate { get; }
    public string PrevCellType { get; }
    public WebPlayer Player { get; }

    public UpdateResponse(string prevCoordinate, string prevCellType, WebPlayer player)
    {
        PrevCoordinate = prevCoordinate;
        PrevCellType = prevCellType;
        Player = player;
    }
}