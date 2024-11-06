using System;

namespace minesweeper.Models;

public interface ICell<T> where T : new()
{
    public event EventHandler? OpenCellEvent;
    public event EventHandler? ToggleFlagEvent;
    public bool HasMine { get; set; }
    public bool IsOpen { get; set; }
    public bool IsFlagged { get; set; }
    public int RowPos { get; set; }
    public int ColumnPos { get; set; }
    public int NeighboringMineCount { get; set; }
}
