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
        var cellTypeRanges = GetCellTypeRanges();
        
        var random = new Random();
        for (var i = 0; i < _cells.GetLength(0); i++)
        {
            for (var j = 0; j < _cells.GetLength(1); j++)
            {
                var randValue = random.NextSingle();

                var cellType = GetCellType(cellTypeRanges, randValue);
                _cells[i, j] = (CellBase)Activator.CreateInstance(cellType!, 
                    new Coordinate(i, j))!;
            }
        }
    }

    private List<(Func<float, bool>, Type)> GetCellTypeRanges()
    {
        var probabilities = new Dictionary<Type, float>
        {
            {typeof(PrizeCell), 0.1f},
            {typeof(StopCell), 0.1f},
            {typeof(WallCell), 0.1f},
            {typeof(TrapCell), 0.1f}
        };

        var cellTypeRanges = new List<(Func<float, bool>, Type)>();

        var counter = 0f;

        foreach (var (cellType, value) in probabilities)
        {
            var minLimit = counter;
            var maxLimit = minLimit + value;
            cellTypeRanges.Add((x => x >= minLimit && x < maxLimit, cellType));
            counter = maxLimit;
        }
        cellTypeRanges.Add((x => x >= counter, typeof(EmptyCell)));
        return cellTypeRanges;
    }

    private Type? GetCellType(List<(Func<float,bool>, Type)>? cellTypeRanges, 
        float rangeValue)
    {
        foreach (var (isInRange, cellType) in cellTypeRanges!)
        {
            if (!isInRange(rangeValue))
            {
                continue;
            }
                    
            return cellType;
        }

        return null;
    }
}