namespace Inertia.GameField;

public class Cell
{
    public CellType Type;

    public Cell(CellType type = CellType.Empty)
    {
        Type = type;
    }
}