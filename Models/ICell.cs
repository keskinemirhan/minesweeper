using Avalonia.Controls;

namespace minesweeper.Models;

public interface ICell 
{
    public bool HasMine { get; set; }
    public bool HasOpened { get; set; }
    public bool IsFlagged { get; set; }
    public int RowPos { get; set; }
    public int ColumnPos { get; set; }
    public int NeighboringMineCount { get; set; }
}
