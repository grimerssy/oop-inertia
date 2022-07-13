using Inertia.Cells;
using Inertia.Domain;

namespace Inertia.Players;

public abstract class Player
{
    private const int DeadEndDepth = 5;
    
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
        if (FilterSafeDirections(Coordinate).Count == 0)
        {
            return false;
        }

        return !IsDeadEnd(Coordinate);
    }

    private bool IsDeadEnd(Coordinate coordinate)
    {
        var seenCoordinates = new List<Coordinate>(new[]{coordinate});

        bool InnerRecursion(Coordinate coord, int iteration = 1)
        {
            if (iteration > DeadEndDepth)
            {
                return false;
            }
            
            var safeDirections = FilterSafeDirections(coord);

            var finalCoordinates = safeDirections.
                Select(d => GetFinalCoordinate(coord, d.Item1, d.Item2)).
                Where(c => !seenCoordinates.Contains(c)).
                ToList();

            if (finalCoordinates.Count == 0)
            {
                return true;
            }

            seenCoordinates.AddRange(finalCoordinates);

            return finalCoordinates.All(c => InnerRecursion(c, iteration + 1));
        }

        return InnerRecursion(coordinate);
    }

    private Coordinate GetFinalCoordinate(Coordinate coordinate, 
        sbyte dX, sbyte dY)
    {
        var (x, y) = (coordinate.X, coordinate.Y);

        while (true)
        {
            x += dX;
            y += dY;

            var cell = _field.GetCell(new Coordinate(x, y));

            if (cell.CanStop)
            {
                return new Coordinate(x, y);
            }
        }
    }
    
    private List<(sbyte, sbyte)> FilterSafeDirections(Coordinate coordinate)
    {
        return Directions.Values.Where(xy => 
                IsDirectionSafe(coordinate, xy.Item1, xy.Item2)).
            ToList();
    }
    
    private bool IsDirectionSafe(Coordinate coordinate, sbyte dX, sbyte dY)
    {
        var (x, y) = (coordinate.X, coordinate.Y);

        while (true)
        {
            x += dX;
            y += dY;

            var cell = _field.GetCell(new Coordinate(x, y));

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