using Inertia.Domain;
using Inertia.GameField;
using Inertia.Players;

namespace Inertia.ConsoleUI;

public class App
{
    private const int AnimationMs = 250;
    
    private const int FieldPadding = 2;
    private const int MessagesPadding = 1;
    private const ConsoleColor TextColor = ConsoleColor.Gray;
    private int MaxPlayers { get; }

    private readonly Field _field;
    private readonly List<ConsolePlayer> _players;

    private readonly Dictionary<CellType, (char, ConsoleColor)> _gameObjects = new()
    {
        {CellType.Empty, (' ', ConsoleColor.Black)},
        {CellType.Prize, ('@', ConsoleColor.Green)},
        {CellType.Stop, ('.', ConsoleColor.Yellow)},
        {CellType.Wall, ('#', ConsoleColor.Yellow)},
        {CellType.Trap, ('%', ConsoleColor.Red)}
    };
    
    public App()
    {
        MaxPlayers = _gameObjects.Count;
        
        var fieldWidth = Console.LargestWindowWidth / 2 - FieldPadding * 2;
        var fieldHeight= Console.LargestWindowHeight - (MaxPlayers + 1) - FieldPadding * 2;
        _field = new Field(fieldWidth, fieldHeight);
        
        _players = InitializePlayers(_field);
    }
    
    private List<ConsolePlayer> InitializePlayers(Field field)
    {
        var playerCount = PromptPlayerCount();
        var getColor = GetColorGenerator();
        var players = new List<ConsolePlayer>();
        
        for (var i = 0; i < playerCount; i++)
        {
            var color = getColor();
            if (color is null)
            {
                return players;
            }
            
            Console.Write("Enter player name: ");
            var name = Console.ReadLine() ?? string.Empty;
            if (name.Equals(string.Empty))
            {
                name = "guest";
            }
            
            var coordinate = GetEmptyCoordinate();
            players.Add(new ConsolePlayer(name, field, coordinate, color.Value));
        }

        return players;
    }

    public void Start()
    {
        var fieldLeft = Console.WindowLeft + FieldPadding;
        var fieldTop = Console.WindowTop + FieldPadding;
        
        var activeButtons = new[]
        {
            'Q', 'W', 'E',
             'A', 'S', 'D',
              'Z',      'C',
            ' '
        };
        
        Console.Clear();
        DisplayField();
        DisplayPlayers();
        DisplayWelcomeMessages();
        
        while (true)
        {
            foreach (var player in _players.Where(p => p.State != PlayerState.Dead))
            {
                Console.ForegroundColor = player.Color;

                var left = Console.WindowLeft + Console.LargestWindowWidth / 2;
                var top = Console.WindowTop + Console.LargestWindowHeight / 2;
                
                Console.SetCursorPosition(left, Console.LargestWindowHeight - top + FieldPadding);
                Console.Write(new string(' ' , Console.LargestWindowWidth - left));
                
                Console.SetCursorPosition(Console.LargestWindowWidth - (left + FieldPadding + player.Name.Length) / 2 , Console.LargestWindowHeight - top + FieldPadding);
                Console.Write($"{player.Name}'s turn");

                var key = GetKey(activeButtons);
                if (key == ' ')
                {
                    return;
                }

                Action<ConsolePlayer> playerAction = key switch
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
                    var (prevX, prevY) = (player.Coordinate.X, player.Coordinate.Y);
                    
                    playerAction(player);
                    playerState = player.State;

                    var cellType = _field.Cells[prevX, prevY].Type;

                    var (symbol, color) = _gameObjects[cellType];

                    Console.ForegroundColor = color;
                    Console.SetCursorPosition(fieldLeft + prevX, fieldTop + prevY);
                    Console.Write(symbol);

                    DisplayPlayers();
                    
                    Task.Delay(AnimationMs).Wait();
                }
                
                Console.ForegroundColor = TextColor;
                Console.SetCursorPosition(Console.WindowLeft, Console.WindowTop);

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

    private void DisplayWelcomeMessages()
    {
        const int controlColumns = 3;
        
        var left = Console.WindowLeft + Console.LargestWindowWidth / 2;
        var top = Console.WindowTop + MessagesPadding;
        
        var controlRows = new[]
        {
            "[Q] Move up-left", "[W] Move up", "[E] Move up-right",
            "[A] Move left", "[S] Move down", "[D] Move right",
            "[Z] Move down-left", "", "[C] Move down-right",
            "[space] Quit", "", ""
        };

        var maxLength = controlRows.Max(r => r.Length);

        controlRows = controlRows.Select(r => string.Format("{0," + -(maxLength + 1) + "}", r)).ToArray();

        var messages = new List<string>(new []{"Welcome to Inertia", "Available actions"});

        if (controlRows.Length % controlColumns != 0)
        {
            throw new Exception("invalid number of table entries");
        }

        for (var i = 0; i < controlRows.Length; i += 3)
        {
            messages.Add(controlRows[i] + controlRows[i+1] + controlRows[i+2]);
        }
        
        for(var i = 0; i < messages.Count; i++)
        {
            var message = messages[i];
            var messageLeft = left + (Console.LargestWindowWidth - left - message.Length) / 2;
            var messageTop = top + (i + 1);
            
            Console.SetCursorPosition(messageLeft, messageTop);
            Console.Write(message);
        }

        Console.SetCursorPosition(Console.WindowLeft, Console.WindowTop);
    }

    private void DisplayField()
    {
        var fieldLeft = Console.WindowLeft + FieldPadding;
        var fieldTop = Console.WindowTop + FieldPadding;
        
        for (var i = 0; i < _field.Cells.GetLength(1); i++)
        {
            Console.SetCursorPosition(fieldLeft,  fieldTop + i);
            for (var j = 0; j < _field.Cells.GetLength(0); j++)
            {
                var (symbol, color) = _gameObjects[_field.Cells[j, i].Type];
                Console.ForegroundColor = color;
                Console.Write(symbol);
            }
        }
        Console.ForegroundColor = TextColor;
    }

    private void DisplayPlayers()
    {
        var fieldLeft = Console.WindowLeft + FieldPadding;
        var fieldTop = Console.WindowTop + FieldPadding;
        var fieldHeight= Console.LargestWindowHeight - (MaxPlayers + 1) - FieldPadding * 2;

        var playersLeft = Console.WindowLeft + MessagesPadding;
        var playersTop = fieldTop + fieldHeight + FieldPadding;

        for (var i = 0; i < _players.Count; i++)
        {
            var player = _players[i];
            Console.ForegroundColor = player.Color;

            var windowThird = (Console.WindowWidth - playersLeft) / 3;
            
            Console.SetCursorPosition(playersLeft, playersTop + i);
            Console.Write("{0,"+ -windowThird +"}", player.Name);
            Console.Write("{0,"+ -windowThird +"}", $"Health: {player.Health}");
            Console.Write("{0,"+ -windowThird +"}", $"Score: {Math.Floor(player.Score)}");

            var (x, y) = (player.Coordinate.X, player.Coordinate.Y);
            Console.SetCursorPosition(fieldLeft + x, fieldTop + y);
            Console.Write('I');
        }
        
        Console.ForegroundColor = TextColor;
        Console.SetCursorPosition(Console.WindowLeft, Console.WindowTop);
    }

    private Func<ConsoleColor?> GetColorGenerator()
    {
        var availableColors = new Stack<ConsoleColor>(new[]
        {
            ConsoleColor.DarkBlue,   ConsoleColor.DarkMagenta,
            ConsoleColor.Magenta,    ConsoleColor.Blue,
            ConsoleColor.Cyan,       ConsoleColor.White
        });

        ConsoleColor? Generator()
        {
            return availableColors.Pop();
        }

        return Generator;
    }

    private int PromptPlayerCount()
    {
        Console.WriteLine("How many players there is going to be?");
        var input = Console.ReadLine() ?? string.Empty;
        if (!int.TryParse(input, out var n))
        {
            n = 1;
        }

        if (n <= MaxPlayers) return n;
        
        Console.WriteLine($"Cannot have more than {MaxPlayers} players at the moment, set player count to {MaxPlayers} \n" +
                          "Press [enter] to continue");
        n = MaxPlayers;
        Console.ReadLine();

        return n;
    }

    private Coordinate GetEmptyCoordinate()
    {
        var random = new Random();
        
        var fieldWidth = Console.LargestWindowWidth / 2 - FieldPadding * 2;
        var fieldHeight = Console.LargestWindowHeight - MaxPlayers - FieldPadding * 3;
        
        while (true)
        {
            var coordinate = new Coordinate(random.Next(0, fieldWidth), random.Next(0, fieldHeight));
            if (_field.GetCellContent(coordinate) == CellType.Empty)
            {
                return coordinate;
            }
        }
    }
}