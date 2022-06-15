using Inertia.Domain;

namespace Inertia.Cells;

public class Cells
{
    private readonly CellBase[,] _cells;

    public int LengthX => _cells.GetLength(0);
    public int LengthY => _cells.GetLength(1);
    
    public Cells(CellBase[,] cells)
    {
        _cells = cells;
    }

    public CellBase this[int x, int y]
    {
        get => _cells[x, y];
        set => _cells[x, y] = value;
    }

    public void FillWithRandomValues()
    {
        var probabilities = new Dictionary<Type, float>
        {
            {typeof(PrizeCell), 0.1f},
            {typeof(StopCell), 0.1f},
            {typeof(WallCell), 0.1f},
            {typeof(TrapCell), 0.1f}
        };

        var filterTypes = new List<(Func<float, bool>, Type)>();

        var counter = 0f;

        foreach (var (cellType, value) in probabilities)
        {
            var minLimit = counter;
            var maxLimit = minLimit + value;
            filterTypes.Add((x => x >= minLimit && x < maxLimit, cellType));
            counter = maxLimit;
        }
        filterTypes.Add((x => x >= counter, typeof(EmptyCell)));
        
        var random = new Random();
        for (var i = 0; i < _cells.GetLength(0); i++)
        {
            for (var j = 0; j < _cells.GetLength(1); j++)
            {
                var randValue = random.NextSingle();

                foreach (var (filter, cellType) in filterTypes)
                {
                    if (filter(randValue))
                    {
                        _cells[i, j] = (CellBase)Activator.CreateInstance(cellType, new Coordinate(i, j))!;
                        break;
                    }
                }
            }
        }
    }
}