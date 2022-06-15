using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public abstract class CellBase
{
    public Coordinate Coordinate { get; }
    
    public bool IsCollectible { get; protected init; }
    public bool IsDangerous { get; protected init; }
    
    public bool CanStop { get; protected init; }
    
    protected CellBase(Coordinate coordinate)
    {
        Coordinate = coordinate;
    }

    public abstract void Interact(Player player);
}