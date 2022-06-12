using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public class StopCell : Cell
{
    public StopCell(Coordinate coordinate) : base(coordinate)
    {
    }

    public override void Interact(Player player)
    {
        player.State = PlayerState.Stopped;
        player.Coordinate = Coordinate;
    }
}