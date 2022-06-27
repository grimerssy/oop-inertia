using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public class PrizeCell : CellBase
{
    public PrizeCell(Coordinate coordinate) : base(coordinate)
    {
        IsCollectible = true;
        IsDangerous = false;
        CanStop = false;
    }

    public override void Interact(Player player)
    {
        player.Score += new Random().NextSingle() * 250;
        player.Coordinate = Coordinate;
        player.RemoveCell(Coordinate);
    }
}