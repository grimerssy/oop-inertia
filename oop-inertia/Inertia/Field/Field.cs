using Inertia.Cells;
using Inertia.Domain;

namespace Inertia.Field;

public class Field
{
    public readonly Cell[,] Cells;

    public Field(int width, int height)
    {
        Cells = new Cell[width, height];
        InitializeCells(Cells);
    }

    public Cell GetCell(Coordinate coordinate)
    {
        var (x, y) = (coordinate.X, coordinate.Y);
        if (x >= Cells.GetLength(0) || x < 0)
        {
            return new WallCell(coordinate);
        }
        if (y >= Cells.GetLength(1) || y < 0)
        {
            return new WallCell(coordinate);
        }

        return Cells[x, y];
    }

    public void RemoveCellIfCollectible(Coordinate coordinate)
    {
        var collectibleTypes = new[] { typeof(PrizeCell), typeof(TrapCell) };
        var (x, y) = (coordinate.X, coordinate.Y);

        var cell = GetCell(coordinate);
        if (collectibleTypes.Contains(cell.GetType()))
        {
            Cells[x, y] = new EmptyCell(cell.Coordinate);
        }
    }

    private void InitializeCells(Cell[,] cells)
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
        for (var i = 0; i < cells.GetLength(0); i++)
        {
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                var randValue = random.NextSingle();

                foreach (var (filter, cellType) in filterTypes)
                {
                    if (filter(randValue))
                    {
                        cells[i, j] = (Cell)Activator.CreateInstance(cellType, new Coordinate(i, j))!;
                        break;
                    }
                }
            }
        }
    }

    public int GetPointsObjective()
    {
        var objective = 0;
        
        for (uint i = 0; i < Cells.GetLength(0); i++)
        {
            for (uint j = 0; j < Cells.GetLength(1); j++)
            {
                if (Cells[i, j].GetType() == typeof(PrizeCell))
                {
                    objective += 50;
                }
            }
        }

        return objective;
    }
    
    public Coordinate GetRandomEmptyCoordinate()
    {
        var random = new Random();
        
        var fieldWidth = Cells.GetLength(0);
        var fieldHeight = Cells.GetLength(1);
        
        while (true)
        {
            var coordinate = new Coordinate(random.Next(0, fieldWidth), random.Next(0, fieldHeight));
            if (GetCell(coordinate).GetType() == typeof(EmptyCell))
            {
                return coordinate;
            }
        }
    }
}
