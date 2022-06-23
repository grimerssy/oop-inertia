using Inertia.Cells;
using Inertia.Domain;
using Inertia.Field;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebApplication1.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class InertiaController
{
    private const string DefaultName = "guest";
    private static Field _field;
    private static WebPlayer[] _players;
    
    [HttpPost]
    public ActionResult<StartResponse> Start([FromBody] StartRequest request)
    {
        _field = new Field(request.FieldWidth, request.FieldHeight);
        
        var colorGenerator = GetHexColorGenerator();
        _players = request.PlayerNames.
            Select(x => string.IsNullOrEmpty(x) ? DefaultName : x).
            Select(x => new WebPlayer(x, _field, _field.GetRandomEmptyCoordinate(), colorGenerator())).
            ToArray();

        var cellTypes = new string[_field.Cells.LengthX][];
        for (var x = 0; x < _field.Cells.LengthX; x++)
        {
            cellTypes[x] = new string[_field.Cells.LengthY];
            for (var y = 0; y < _field.Cells.LengthY; y++)
            {
                var cell = _field.Cells[x, y];

                cellTypes[x][y] = GetCellName(cell);
            }
        }

        return new StartResponse(cellTypes, _players);
    }

    [HttpPut]
    [Route("{playerName}/{direction}")]
    public ActionResult<UpdateResponse> Update(string playerName, string direction)
    {
        var player = _players.First(p => p.Name == playerName);
        var prevCoordinate = player.Coordinate;
        
        switch (direction.ToUpper()) 
        {
            case "TL":
                player.Move(Direction.TopLeft); 
                break;
            case "T":
                player.Move(Direction.Top); 
                break; 
            case "TR": 
                player.Move(Direction.TopRight); 
                break; 
            case "L": 
                player.Move(Direction.Left); 
                break; 
            case "R": 
                player.Move(Direction.Right); 
                break; 
            case "BL": 
                player.Move(Direction.BottomLeft); 
                break; 
            case "B":
                player.Move(Direction.Bottom); 
                break;
            case "BR": 
                player.Move(Direction.BottomRight);
                break;
            default:
                return new BadRequestResult();
        }
        
        var response = new UpdateResponse(
            prevCoordinate.X + ":" + prevCoordinate.Y, 
            GetCellName(_field.GetCell(prevCoordinate)),
            player);

        return response;
    }

    private Func<string> GetHexColorGenerator()
    {
        var colors = new Queue<string> (new [] {
            "#FF0000", "#00FF00", "#0000FF", "#FFFF00", "#FF00FF", "#00FFFF",
            "#800000", "#008000", "#000080", "#808000", "#800080", "#008080",
            "#C00000", "#00C000", "#0000C0", "#C0C000", "#C000C0", "#00C0C0", 
            "#400000", "#004000", "#000040", "#404000", "#400040", "#004040", 
            "#200000", "#002000", "#000020", "#202000", "#200020", "#002020", 
            "#600000", "#006000", "#000060", "#606000", "#600060", "#006060", 
            "#A00000", "#00A000", "#0000A0", "#A0A000", "#A000A0", "#00A0A0", 
            "#E00000", "#00E000", "#0000E0", "#E0E000", "#E000E0", "#00E0E0"
        });
    
        return () => colors.Dequeue();
    }

    private string GetCellName(CellBase cell)
    {
        return cell switch
        {
            EmptyCell => "empty",
            PrizeCell => "prize",
            StopCell => "stop",
            TrapCell => "trap",
            WallCell => "wall",
            _ => throw new InvalidDataException("did not match cell type")
        };
    }
}