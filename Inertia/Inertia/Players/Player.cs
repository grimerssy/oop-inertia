using Inertia.Domain;
using Inertia.GameField;

namespace Inertia.Players;

public class Player
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
    
    public float Health { get; private set; } = 100f;
    public double Score { get; private set; } = 0;
    
    public Player(Field field, Coordinate coordinate)
    {
        _field = field;
        Coordinate = coordinate;
    }
    
    public void Move(Direction direction)
    {
        var (x, y) = (Coordinate.X, Coordinate.Y);
        var (dX, dY) = Directions[direction];

        x += dX;
        y += dY;

        if (x == -1 || y == -1)
        {
            State = PlayerState.Stopped;
            return;
        }

        var coordinate = new Coordinate(x, y);
        var cellType = _field.GetCellType(coordinate);

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
    
}