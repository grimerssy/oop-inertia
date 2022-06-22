using Inertia.Cells;
using Inertia.Domain;

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
    
    private readonly Field.Field _field;
    public Coordinate Coordinate { get; set; }
    public PlayerState State { get; set; }

    public string Name { get; }
    public float Health { get; set; } = 100f;
    public double Score { get; set; } = 0;

    protected Player(string name, Field.Field field, Coordinate coordinate)
    {
        Name = name;
        _field = field;
        Coordinate = coordinate;
    }
    
    public void Move(Direction direction)
    {
        State = PlayerState.Moving;
        
        if (!CanMove())
        {
            State = PlayerState.Stopped;
            Coordinate = _field.GetRandomEmptyCoordinate();
            return;
        }
        
        var (x, y) = (Coordinate.X, Coordinate.Y);
        var (dX, dY) = Directions[direction];

        x += dX;
        y += dY;
        
        var coordinate = new Coordinate(x, y);
        var cell = _field.GetCell(coordinate);
        cell.Interact(this);

        if (Health <= 0)
        {
            State = PlayerState.Dead;
        }
    }

    public void RemoveCell(Coordinate coordinate)
    {
        var emptyCell = new EmptyCell(Coordinate);
        _field.ReplaceCell(coordinate, emptyCell);
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

        if (adjacentCoordinates.All(c => _field.GetCell(c).GetType() == typeof(WallCell)))
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
            var (newX, newY) = (x + dX, y + dY);

            while (true)
            {
                newX += dX;
                newY += dY;

                var coordinate = new Coordinate(newX, newY);
                var cell = _field.GetCell(coordinate);

                if (cell.IsDangerous)
                {
                    return false;
                }
                
                if (cell.CanStop)
                {
                    return true;
                }
            }
        }
    }
}