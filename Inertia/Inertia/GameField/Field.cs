using Inertia.Domain;

namespace Inertia.GameField;

public class Field
{
    public readonly Cell[,] Cells;

    public Field(int width, int height)
    {
        Cells = new Cell[width, height];
        InitializeCells(Cells);
    }

    public CellType GetCellType(Coordinate coordinate)
    {
        var collectibleTypes = new[] { CellType.Prize, CellType.Trap };
        
        var (x, y) = (coordinate.X, coordinate.Y);
        if (x >= Cells.GetLength(0) || x < 0)
        {
            return CellType.Wall;
        }
        if (y >= Cells.GetLength(1) || y < 0)
        {
            return CellType.Wall;
        }

        var type = Cells[x, y].Type;
        if (collectibleTypes.Contains(type))
        {
            Cells[x, y].Type = CellType.Empty;
        }

        return type;
    }

    private void InitializeCells(Cell[,] cells)
    {
        var probabilities = new Dictionary<CellType, float>
        {
            {CellType.Prize, 0.1f},
            {CellType.Stop, 0.1f},
            {CellType.Wall, 0.1f},
            {CellType.Trap, 0.1f}
        };

        var typeFilter = new Dictionary<Func<float, bool>, CellType>();
        var counter = 0f;

        foreach (var (cellType, value) in probabilities)
        {
            var minLimit = counter;
            var maxLimit = minLimit + value;
            typeFilter.Add(x => x >= minLimit && x < maxLimit, cellType);
            counter = maxLimit;
        }

        typeFilter.Add(x => x >= counter, CellType.Empty);

        var random = new Random();
        for (uint i = 0; i < cells.GetLength(0); i++)
        {
            for (uint j = 0; j < cells.GetLength(1); j++)
            {
                var randValue = random.NextSingle();
                var cellType = typeFilter
                    .Where(x => x.Key(randValue))
                    .Select(x => x.Value)
                    .First();

                cells[i, j] = new Cell(cellType);
            }
        }
    }
}