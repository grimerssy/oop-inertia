using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public class TrapCell : Cell
{
    public TrapCell(Coordinate coordinate) : base(coordinate)
    {
    }

    public override void Interact(Player player)
    {
        player.Health -= 100;
        player.Coordinate = Coordinate;
    }
}