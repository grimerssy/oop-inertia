using Inertia.Domain;
using Inertia.Players;

namespace ConsoleUI;

public class ConsolePlayer : Player
{
    public readonly ConsoleColor Color;
    
    public ConsolePlayer(string name, Inertia.Field.Field field, Coordinate coordinate, ConsoleColor color) : base(name, field, coordinate)
    {
        Color = color;
    }
}