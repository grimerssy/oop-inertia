using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public abstract class Cell
{
    public Coordinate Coordinate { get; }
    
    protected Cell(Coordinate coordinate)
    {
        Coordinate = coordinate;
    }

    public abstract void Interact(Player player);
}