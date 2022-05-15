using Inertia.Domain;
using Inertia.GameField;
using Inertia.Players;

namespace Inertia.consoleUI;

public class App
{
    private const int FieldHeight = 30;
    private const int FieldWidth = 50;
    private const ConsoleColor DefaultColor = ConsoleColor.Gray;
    private readonly Field _field;
    private readonly List<Player> _players;
    private readonly Dictionary<Player, ConsoleColor> _playerColors = new();

    public App()
    {
        _field = new(FieldWidth, FieldHeight);

        var playerCount = PromptPlayerCount();
        _players = new List<Player>();
        for (var i = 0; i < playerCount; i++)
        {
            var coordinate = GetEmptyCoordinate();
            _players.Add(new Player(_field, coordinate));
        }
        AssignColors();
    }

    public void Start()
    {
        var activeButtons = new[]
        {
            'Q', 'W', 'E',
             'A', 'S', 'D',
              'Z',      'C',
            ' '
        };
        
        while (true)
        {
            foreach (var player in _players.Where(p => p.State != PlayerState.Dead))
            {
                Render();
                
                var (cursorX, cursorY) = Console.GetCursorPosition();
                var color = _playerColors[player];
                Console.ForegroundColor = color;
                Console.SetCursorPosition(FieldWidth + 35, 20);
                Console.Write($"{color.ToString()}'s turn");
                Console.ForegroundColor = DefaultColor;
                Console.SetCursorPosition(cursorX, cursorY);
                
                var key = GetKey(activeButtons);
                if (key == ' ')
                {
                    return;
                }

                Action<Player> playerAction = key switch
                {
                    'Q' => p => p.Move(Direction.TopLeft),
                    'W' => p => p.Move(Direction.Top),
                    'E' => p => p.Move(Direction.TopRight),
                    'A' => p => p.Move(Direction.Left),
                    'S' => p => p.Move(Direction.Bottom),
                    'D' => p => p.Move(Direction.Right),
                    'Z' => p => p.Move(Direction.BottomLeft),
                    'C' => p => p.Move(Direction.BottomRight),
                    _ => throw new ArgumentException()
                };

                var playerState = PlayerState.Moving;

                while (playerState == PlayerState.Moving)
                {
                    playerAction(player);
                    playerState = player.State;
                    Render();
                    Task.Delay(300).Wait();
                }

                if (_players.All(p => p.State == PlayerState.Dead))
                {
                    return;
                }
            }
        }
    }

    private char GetKey(ICollection<char> accept)
    {
        while (true)
        {
            var key = char.ToUpper(Console.ReadKey(true).KeyChar);
            if (accept.Contains(key))
            {
                return key;
            }
        }
    }

    private void Render()
    {
        Console.Clear();
        DisplayWelcomeMessage();
        DisplayField();
        DisplayPlayers();
    }

    private void DisplayWelcomeMessage()
    {
        Console.ForegroundColor = DefaultColor;
        Console.SetCursorPosition(FieldWidth + 34, 1);
        Console.WriteLine("Welcome to Inertia");
        Console.SetCursorPosition(FieldWidth + 34, 3);
        Console.WriteLine("Available actions");
        Console.SetCursorPosition(FieldWidth, 5);
        Console.WriteLine("\t\t Move to: [Q] Up-Left \t [W] Up \t [E] Up-Right");
        Console.SetCursorPosition(FieldWidth, 6);
        Console.WriteLine("\t\t\t  [A] Left \t [S] Down \t [D] Right");
        Console.SetCursorPosition(FieldWidth, 7);
        Console.WriteLine("\t\t\t  [Z] Down-Left \t\t [C] Down-Right");
        Console.SetCursorPosition(FieldWidth, 9);
        Console.WriteLine("\t\t To quit press the [space] key");
    }

    private void DisplayField()
    {
        Console.SetCursorPosition(1, 1);
        var gameObjects = new Dictionary<CellType, (char, ConsoleColor)>
        {
            {CellType.Empty, (' ', ConsoleColor.Black)},
            {CellType.Prize, ('@', ConsoleColor.Green)},
            {CellType.Stop, ('.', ConsoleColor.Yellow)},
            {CellType.Wall, ('#', ConsoleColor.Yellow)},
            {CellType.Trap, ('%', ConsoleColor.Red)}
        };

        for (var i = 0; i < _field.Cells.GetLength(1); i++)
        {
            Console.SetCursorPosition(1,  i + 1);
            for (var j = 0; j < _field.Cells.GetLength(0); j++)
            {
                var (symbol, color) = gameObjects[_field.Cells[j, i].Type];
                Console.ForegroundColor = color;
                Console.Write(symbol);
            }
        }
        Console.WriteLine('\n');
        Console.ForegroundColor = DefaultColor;
    }

    private void DisplayPlayers()
    {
        foreach (var player in _players)
        {
            var color = _playerColors[player];
                Console.ForegroundColor = color;
            
            Console.WriteLine($" {color.ToString()}    \t Health: {(int)player.Health} \t Score: {(int)player.Score}");

            var coordinate = player.Coordinate;
            var (x, y) = (coordinate.X, coordinate.Y);
            var (prevX, prevY) = Console.GetCursorPosition();
            Console.SetCursorPosition(  x + 2,  y + 1);
            Console.Write("\bI");
            Console.SetCursorPosition(prevX, prevY);
        }
        Console.ForegroundColor = DefaultColor;
    }

    private void AssignColors()
    {
        var availableColors = new Stack<ConsoleColor>(new[]
        {
            ConsoleColor.DarkBlue,   ConsoleColor.DarkMagenta,
            ConsoleColor.Magenta,    ConsoleColor.Blue,
            ConsoleColor.Cyan,       ConsoleColor.White
        });

        foreach (var player in _players)
        {
            if (availableColors.Count == 0)
            {
                return;
            }
            _playerColors.Add(player, availableColors.Pop());
        }
    }

    private int PromptPlayerCount()
    {
        Console.WriteLine("How many players there is going to be?");
        var input = Console.ReadLine() ?? string.Empty;
        if (!int.TryParse(input, out var n))
        {
            n = 1;
        }

        if (n <= 6) return n;
        
        Console.WriteLine("Cannot have more than 6 players at the moment, set player count to 6 \n" +
                          "Press [enter] to continue");
        n = 6;
        Console.ReadLine();

        return n;
    }

    private Coordinate GetEmptyCoordinate()
    {
        var random = new Random();
        while (true)
        {
            var coordinate = new Coordinate(random.Next(0, FieldWidth), random.Next(0, FieldHeight));
            if (_field.GetCellType(coordinate) == CellType.Empty)
            {
                return coordinate;
            }
        }
    }
}