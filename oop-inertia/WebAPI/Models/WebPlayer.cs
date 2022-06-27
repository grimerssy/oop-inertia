using Inertia.Domain;
using Inertia.Field;
using Inertia.Players;

namespace WebAPI.Models;

public class WebPlayer : Player
{
    public string Color { get; }

    public WebPlayer(string name, Field field, Coordinate coordinate, string color) : base(name, field, coordinate)
    {
        Color = color;
    }
}