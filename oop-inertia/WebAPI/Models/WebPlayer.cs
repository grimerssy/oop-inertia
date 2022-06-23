using Inertia.Domain;
using Inertia.Field;
using Inertia.Players;

namespace WebApplication1.Models;

public class WebPlayer : Player
{
    public readonly string ColorHex;

    public WebPlayer(string name, Field field, Coordinate coordinate, string colorHex) : base(name, field, coordinate)
    {
        ColorHex = colorHex;
    }
}