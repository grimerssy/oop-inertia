using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public class EmptyCell : CellBase
{
    public EmptyCell(Coordinate coordinate) : base(coordinate)
    {
        IsCollectible = false;
        IsDangerous = false;
        CanStop = false;
    }

    public override void Interact(Player player)
    {
        player.Coordinate = Coordinate;
    }
}