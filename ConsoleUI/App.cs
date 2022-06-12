using Inertia.Cells;
using Inertia.Domain;
using Inertia.Players;

namespace ConsoleUI;

public class App
{
    private const int MaxAnimationMs = 400;
    private const int AccelerationPercentage = 15;
    private const int MinAnimationMs = 100;
    
    private const int MaxPlayers = 6;
    
    private const int FieldPadding = 2;
    private const int MessagesPadding = 5;
    private const ConsoleColor TextColor = ConsoleColor.Gray;

    private readonly int _pointsObjective;
    
    private readonly int _fieldWidth;
    private readonly int _fieldHeight;
    private readonly int _fieldLeft;
    private readonly int _fieldTop;

    private readonly Inertia.Field.Field _field;
    private readonly List<ConsolePlayer> _players;

    private readonly Dictionary<Type, (char, ConsoleColor)> _gameObjects = new()
    {
        {typeof(EmptyCell), (' ', ConsoleColor.Black)},
        {typeof(PrizeCell), ('@', ConsoleColor.Green)},
        {typeof(StopCell), ('.', ConsoleColor.Yellow)},
        {typeof(WallCell), ('#', ConsoleColor.Yellow)},
        {typeof(TrapCell), ('%', ConsoleColor.Red)}
    };
    
    public App()
    {
        _fieldWidth = Console.LargestWindowWidth / 2 - FieldPadding * 2;
        _fieldHeight = Console.LargestWindowHeight - (MaxPlayers + 2) - FieldPadding * 2;
        _fieldLeft = Console.WindowLeft + FieldPadding;
        _fieldTop = Console.WindowTop + FieldPadding;
        
        _field = new Inertia.Field.Field(_fieldWidth, _fieldHeight);
        _pointsObjective = _field.GetPointsObjective();
        
        _players = InitializePlayers(_field);
    }
    
    private List<ConsolePlayer> InitializePlayers(Inertia.Field.Field field)
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
            var input = Console.ReadLine();
            
            var name = string.Equals(input, string.Empty) ? "guest" : input; 
            
            var coordinate = _field.GetRandomEmptyCoordinate();
            players.Add(new ConsolePlayer(name, field, coordinate, color.Value));
        }

        return players;
    }

    public void Start()
    {
        Console.SetCursorPosition(Console.LargestWindowWidth, Console.LargestWindowHeight);
        Console.Write(' ');
        
        Console.Clear();
        DisplayField();
        DisplayPlayers();
        DisplayWelcomeMessages();
        
        while (true)
        {
            foreach (var player in _players.Where(p => p.State != PlayerState.Dead))
            {
                var activeButtons = new[]
                {
                    'Q', 'W', 'E',
                    'A', 'S', 'D',
                    'Z',      'C',
                    ' '
                };
                
                DisplayPlayerTurn(player);

                var key = GetKey(activeButtons);
                if (key == ' ')
                {
                    return;
                }

                ApplyActionToPlayer(player, key);

                if (_players.Any(p => p.State != PlayerState.Dead) &&
                    _players.All(p => p.Score < _pointsObjective))
                {
                    continue;
                }
                
                var winner = _players.MaxBy(p => p.Score);

                Console.Clear();
                Console.ForegroundColor = winner.Color;
                Console.WriteLine($"{winner.Name} won!");
                return;
            }
        }
    }

    private void ApplyActionToPlayer(ConsolePlayer player, char key)
    {
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
        var delay = MaxAnimationMs;

        while (playerState == PlayerState.Moving)
        {
            var (prevX, prevY) = (player.Coordinate.X, player.Coordinate.Y);

            playerAction(player);
            playerState = player.State;

            var cellType = _field.Cells[prevX, prevY].GetType();

            var (symbol, color) = _gameObjects[cellType];

            Console.ForegroundColor = color;
            Console.SetCursorPosition(_fieldLeft + prevX, _fieldTop + prevY);
            Console.Write(symbol);

            DisplayPlayers();

            Task.Delay(delay).Wait();

            var newDelay = delay - delay * AccelerationPercentage / 100;
            delay = Math.Min(newDelay, MinAnimationMs);
        }

        Console.ForegroundColor = TextColor;
        Console.SetCursorPosition(Console.WindowLeft, Console.WindowTop);
    }

    private static void DisplayPlayerTurn(ConsolePlayer player)
    {
        Console.ForegroundColor = player.Color;

        var left = Console.WindowLeft + Console.LargestWindowWidth / 2;
        var top = Console.WindowTop + Console.LargestWindowHeight / 2;

        Console.SetCursorPosition(left, Console.LargestWindowHeight - top + FieldPadding);
        Console.Write(new string(' ', Console.LargestWindowWidth - left));

        Console.SetCursorPosition(Console.LargestWindowWidth - (left + FieldPadding + player.Name.Length) / 2,
            Console.LargestWindowHeight - top + FieldPadding);
        Console.Write($"{player.Name}'s turn");
        Console.SetCursorPosition(Console.WindowLeft, Console.WindowTop);
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
            "[space] Quit", "", "",
            "", "", "",
            "", $"Objective: {_pointsObjective} points", ""
        };

        var maxLength = controlRows.Max(r => r.Length);

        controlRows = controlRows.Select(r => string.Format("{0," + -(maxLength + 1) + "}", r)).ToArray();

        var messages = new List<string>(new []{"Welcome to Inertia", "", "Available actions", ""});

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
        
        Console.SetCursorPosition(MessagesPadding, Console.LargestWindowHeight);
        Console.Write("note: if player cannot move they will be teleported and will be able to move on the next turn");

        Console.SetCursorPosition(Console.WindowLeft, Console.WindowTop);
    }

    private void DisplayField()
    {
        for (var i = 0; i < _field.Cells.GetLength(1); i++)
        {
            Console.SetCursorPosition(_fieldLeft,  _fieldTop + i);
            for (var j = 0; j < _field.Cells.GetLength(0); j++)
            {
                var (symbol, color) = _gameObjects[_field.Cells[j, i].GetType()];
                Console.ForegroundColor = color;
                Console.Write(symbol);
            }
        }
        Console.ForegroundColor = TextColor;
    }

    private void DisplayPlayers()
    {
        var playersLeft = Console.WindowLeft + FieldPadding;
        var playersTop = _fieldTop + _fieldHeight + FieldPadding;

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
            Console.SetCursorPosition(_fieldLeft + x, _fieldTop + y);
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
            try
            {
                return availableColors.Pop();
            }
            catch (InvalidOperationException)
            {
                Console.Clear();
                Console.WriteLine("Unexpected error occurred, reach out to stanislav.stoianov@nure.ua to report a problem");
                throw;
            }
        }

        return Generator;
    }

    private int PromptPlayerCount()
    {
        Console.WriteLine("How many players there is going to be?");
        var input = Console.ReadLine() ?? string.Empty;
        if (!int.TryParse(input, out var n) || n < 1)
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
}