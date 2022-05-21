using Inertia.Domain;
using Inertia.GameField;

namespace Inertia.Players;

public abstract class Player
{
    private static readonly IReadOnlyDictionary<Direction, (sbyte, sbyte)> Directions =
        new Dictionary<Direction, (sbyte, sbyte)>
        {
            {Direction.Top, (0, -1)},
            {Direction.TopRight, (1, -1)},
            {Direction.Right, (1, 0)},
            {Direction.BottomRight, (1, 1)},
            {Direction.Bottom, (0, 1)},
            {Direction.BottomLeft, (-1, 1)},
            {Direction.Left, (-1, 0)},
            {Direction.TopLeft, (-1, -1)}
        };
    
    private readonly Field _field;
    public Coordinate Coordinate { get; private set; }
    public PlayerState State { get; private set; }

    public string Name { get; }
    public float Health { get; private set; } = 100f;
    public double Score { get; private set; } = 0;

    protected Player(string name, Field field, Coordinate coordinate)
    {
        Name = name;
        _field = field;
        Coordinate = coordinate;
    }
    
    public void Move(Direction direction)
    {
        if (!CanMove())
        {
            State = PlayerState.Stopped;
            Coordinate = _field.GetEmptyCoordinate();
            return;
        }
        
        var (x, y) = (Coordinate.X, Coordinate.Y);
        var (dX, dY) = Directions[direction];

        x += dX;
        y += dY;
        
        var coordinate = new Coordinate(x, y);
        var cellType = _field.GetCellContent(coordinate);

        switch (cellType)
        {
            case CellType.Wall:
                State = PlayerState.Stopped;
                return;
            
            case CellType.Stop:
                Coordinate = coordinate;
                State = PlayerState.Stopped;
                return;
            
            case CellType.Trap:
                Health -= 100;
                break;
            
            case CellType.Prize:
                Score += new Random().NextSingle() * 1000;
                break;
        }
        Coordinate = coordinate;
            
        State = Health <= 0 ? PlayerState.Dead : PlayerState.Moving;
    }

    private bool CanMove()
    {
        var (x, y) = (Coordinate.X, Coordinate.Y);

        var adjacentCoordinates = new[]
        {
            new Coordinate(x - 1, y - 1),
            new Coordinate(x - 1, y),
            new Coordinate(x - 1, y + 1),
            new Coordinate(x, y - 1),
            new Coordinate(x, y + 1),
            new Coordinate(x + 1, y - 1),
            new Coordinate(x + 1, y),
            new Coordinate(x + 1, y + 1)
        };

        if (adjacentCoordinates.All(c => _field.GetCellContent(c) == CellType.Wall))
        {
            return false;
        }

        foreach (var direction in Directions)
        {
            var (dX, dY) = direction.Value;

            if (IsDirectionSafe(dX, dY))
            {
                return true;
            }
        }
        
        return false;

        bool IsDirectionSafe(sbyte dX, sbyte dY)
        {
            var avoidCells = new[]
            {
                CellType.Trap
            };
            
            var stopCells = new[]
            {
                CellType.Stop,
                CellType.Wall
            };

            var (newX, newY) = (x + dX, y + dY);

            while (true)
            {
                newX += dX;
                newY += dY;

                var coordinate = new Coordinate(newX, newY);
                var cellType = _field.GetCellContent(coordinate);

                if (avoidCells.Contains(cellType))
                {
                    return false;
                }
                
                if (stopCells.Contains(cellType))
                {
                    return true;
                }
            }
        }
    }
}