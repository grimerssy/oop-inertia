using Inertia.Cells;
using Inertia.Domain;
using Inertia.Field;
using Inertia.Storage;
using WebAPI.Models;

namespace WebAPI.Services;

public class InertiaService
{
    private const string DefaultName = "guest";
    private static Field _field;
    private static WebPlayer[] _players;
    private readonly BestScoresStorage _bestScoresStorage;

    public InertiaService(BestScoresStorage bestScoresStorage)
    {
        _bestScoresStorage = bestScoresStorage;
    }

    public StartResponse Start(StartRequest request)
    {
        _field = new Field(request.FieldWidth, request.FieldHeight);
        
        var colorGenerator = GetHexColorGenerator();
        _players = request.PlayerNames.
            Select(x => string.IsNullOrEmpty(x) ? DefaultName : x).
            Select(x => new WebPlayer(x, _field, 
                _field.GetRandomEmptyCoordinate(), colorGenerator())).
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

        var pointsObjective = _field.GetPointsObjective();

        return new StartResponse(pointsObjective, cellTypes, _players);
    }
    
    public UpdateResponse Update(string playerColor, string directionCode)
    {
        var directions = new Dictionary<string, Direction>()
        {
            {"TL", Direction.TopLeft},
            {"T", Direction.Top},
            {"TR", Direction.TopRight},
            {"L", Direction.Left},
            {"R", Direction.Right},
            {"BL", Direction.BottomLeft},
            {"B", Direction.Bottom},
            {"BR", Direction.BottomRight},
        };
        
        playerColor = "#" + playerColor.ToUpper();
        var player = _players.First(p => p.Color == playerColor);
        var prevCoordinate = player.Coordinate;

        var direction = directions[directionCode.ToUpper()];
        player.Move(direction);
        
        var response = new UpdateResponse(
            prevCoordinate.X + ":" + prevCoordinate.Y, 
            GetCellName(_field.GetCell(prevCoordinate)),
            player);

        return response;
    }
    
    public void SaveResults()
    {
        foreach (var player in _players.Where(p => p.Name != DefaultName))
        {
            _bestScoresStorage.Add(player.Name, Convert.ToInt32(player.Score));
        }
    }
    
    public LeaderboardEntry[] GetTopResults(int count)
    {
        var list = new List<LeaderboardEntry>();
        foreach (var (name, score) in _bestScoresStorage.GetTopScores(count))
        {
            list.Add(new LeaderboardEntry(name, score));
        }

        return list.ToArray();
    }
    
    private Func<string> GetHexColorGenerator()
    {
        var colors = new Queue<string> (new [] {
            "#FF0000", "#0000FF", "#008000", "#800000", "#808000", "#800080",
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