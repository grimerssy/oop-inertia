using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public class TrapCell : CellBase
{
    public TrapCell(Coordinate coordinate) : base(coordinate)
    {
        IsCollectible = true;
        IsDangerous = true;
        CanStop = false;
    }

    public override void Interact(Player player)
    {
        player.Health -= 100;
        player.Coordinate = Coordinate;
        player.RemoveCell(Coordinate);
    }
}