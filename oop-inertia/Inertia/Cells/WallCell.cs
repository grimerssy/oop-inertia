using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public class WallCell : Cell
{
    public WallCell(Coordinate coordinate) : base(coordinate)
    {
    }

    public override void Interact(Player player)
    {
        player.State = PlayerState.Stopped;
    }
}