using Inertia.Domain;
using Inertia.Players;

namespace Inertia.Cells;

public class PrizeCell : Cell
{
    public PrizeCell(Coordinate coordinate) : base(coordinate)
    {
    }

    public override void Interact(Player player)
    {
        player.Score += new Random().NextSingle() * 1000;
        player.Coordinate = Coordinate;
    }
}