using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public class EmptyCell : Cell
{
    public EmptyCell(Coordinate coordinate) : base(coordinate)
    {
    }

    public override void Interact(Player player)
    {
        player.Coordinate = Coordinate;
    }
}