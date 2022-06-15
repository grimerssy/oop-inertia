using Inertia.Cells;
using Inertia.Domain;

namespace Inertia.Field;

public class Field
{
    public readonly Cells.Cells Cells;

    public Field(int width, int height)
    {
        Cells = new Cells.Cells(new CellBase[width, height]);
        Cells.FillWithRandomValues();
    }

    public CellBase GetCell(Coordinate coordinate)
    {
        var (x, y) = (coordinate.X, coordinate.Y);
        if (x >= Cells.LengthX || x < 0)
        {
            return new WallCell(coordinate);
        }
        if (y >= Cells.LengthY || y < 0)
        {
            return new WallCell(coordinate);
        }

        return Cells[x, y];
    }

    public void RemoveCellIfCollectible(Coordinate coordinate)
    {
        var (x, y) = (coordinate.X, coordinate.Y);

        var cell = GetCell(coordinate);
        if (cell.IsCollectible)
        {
            Cells[x, y] = new EmptyCell(cell.Coordinate);
        }
    }

    public int GetPointsObjective()
    {
        var objective = 0;
        
        for (var i = 0; i < Cells.LengthX; i++)
        {
            for (var j = 0; j < Cells.LengthY; j++)
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
        
        var fieldWidth = Cells.LengthX;
        var fieldHeight = Cells.LengthY;
        
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
