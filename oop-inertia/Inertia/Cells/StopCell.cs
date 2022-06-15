using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public class StopCell : CellBase
{
    public StopCell(Coordinate coordinate) : base(coordinate)
    {
        IsCollectible = false;
        IsDangerous = false;
        CanStop = true;
    }

    public override void Interact(Player player)
    {
        player.State = PlayerState.Stopped;
        player.Coordinate = Coordinate;
    }
}