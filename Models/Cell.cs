namespace minesweeper.Models;

public class Cell
{

    public bool HasMine { get; set; } = false;
    public bool HasOpened { get; set; } = false;
    public bool IsFlagged { get; set; } = false;
    public int RowPos { get; set; }
    public int ColumnPos { get; set; }
    public int NeighboringMineCount { get; set; } = 0;

    public Cell(int rowPos, int columnPos)
    {
        this.ColumnPos = columnPos;
        this.RowPos = rowPos;
    }
}