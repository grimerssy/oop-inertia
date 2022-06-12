using Inertia.Domain;
using Inertia.Players;

namespace Inertia.ConsoleUI;

public class ConsolePlayer : Player
{
    public ConsoleColor Color;
    
    public ConsolePlayer(string name, Field.Field field, Coordinate coordinate, ConsoleColor color) : base(name, field, coordinate)
    {
        Color = color;
    }
}