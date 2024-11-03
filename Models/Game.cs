using System;
using System.Collections.Generic;

namespace minesweeper.Models;

public class Game<T> where T : ICell, new()
{
    public int RowCount { get; private set; }
    public int ColumnCount { get; private set; }
    public int NonOpenedCellCount { get; private set; }
    public int FlaggedCellCount { get; private set; }
    public int MineCount { get; private set; }

    public bool IsOver { get; private set; }
    public bool HasWon { get; private set; }

    private T[,] cells;

    public T GetCell(int rowPos, int columnPos)
    {
        return this.cells[rowPos, columnPos];
    }

    public void OpenCell(int rowPos, int columnPos)
    {
        if (
            rowPos >= this.RowCount ||
            columnPos >= this.ColumnCount ||
            rowPos < 0 ||
            columnPos < 0
        ) return;
        var cell = this.GetCell(rowPos, columnPos);
        this.OpenCell(cell);
    }

    public void OpenCell(T cell)
    {
        if (this.IsOver) return;
        if (cell.IsFlagged) this.UnFlagCell(cell);
        if (cell.HasMine)
        {
            cell.HasOpened = true;
            this.IsOver = true;
            this.HasWon = false;
            return;
        }
        var openCells = new List<T>();
        this.openCellRecursive(cell, openCells);
        this.NonOpenedCellCount -= openCells.Count;
        Console.WriteLine("NonOpened: " + this.NonOpenedCellCount);
        if (this.NonOpenedCellCount <= 0)
        {
            this.IsOver = true;
            this.HasWon = true;
        }
    }

    private void openCellRecursive(T cell, List<T> openCells)
    {
        if (cell.HasOpened) return;
        cell.HasOpened = true;
        openCells.Add(cell);
        if (cell.NeighboringMineCount == 0)
        {
            foreach (var neighbor in this.GetNeighbors(cell.RowPos, cell.ColumnPos))
            {
                if (!neighbor.HasOpened)
                {
                    this.openCellRecursive(neighbor, openCells);
                }
            }
        }
    }


    public void FlagCell(int rowPos, int columnPos)
    {
        var cell = this.GetCell(rowPos, columnPos);
        this.FlagCell(cell);
    }

    public void FlagCell(T cell)
    {
        if (this.IsOver || cell.HasOpened) return;
        cell.IsFlagged = true;
    }


    public void UnFlagCell(int rowPos, int columnPos)
    {
        var cell = this.GetCell(rowPos, columnPos);
        this.UnFlagCell(cell);
    }

    public void UnFlagCell(T cell)
    {
        if (this.IsOver) return;
        cell.IsFlagged = false;
    }

    public List<T> GetMineCells()
    {
        var mineCells = new List<T>();
        foreach (var cell in this.cells)
        {
            if (cell.HasMine) mineCells.Add(cell);
        }
        return mineCells;
    }
    public Game(int rowCount, int columnCount, int mineCount)
    {
        this.RowCount = rowCount;
        this.ColumnCount = columnCount;
        this.MineCount = mineCount;

        this.cells = this.GenerateCells();
        this.NonOpenedCellCount = rowCount * columnCount - mineCount;
        this.FlaggedCellCount = 0;

        this.GenerateMines(mineCount);
        this.CountNeighborMines();
    }
    private void GenerateMines(int count)
    {
        var random = new Random();
        for (int i = 0; i < count; i++)
        {
            int rowPos;
            int columnPos;
            do
            {
                columnPos = random.Next(this.ColumnCount);
                rowPos = random.Next(this.RowCount);

            } while (this.cells[columnPos, rowPos].HasMine);
            this.cells[columnPos, rowPos].HasMine = true;
        }
    }
    private void CountNeighborMines()
    {
        for (int rowPos = 0; rowPos < this.RowCount; rowPos++)
        {
            for (int columnPos = 0; columnPos < this.ColumnCount; columnPos++)
            {
                var cell = this.cells[rowPos, columnPos];
                var neighbors = this.GetNeighbors(rowPos, columnPos);
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.HasMine) cell.NeighboringMineCount++;
                }
            }
        }
    }
    private T[,] GenerateCells()
    {
        var genCells = new T[this.RowCount, this.ColumnCount];
        for (int rowPos = 0; rowPos < this.RowCount; rowPos++)
        {
            for (int columnPos = 0; columnPos < this.ColumnCount; columnPos++)
            {
                var cell = new T();
                cell.RowPos = rowPos;
                cell.ColumnPos = columnPos;
                cell.HasMine = false;
                cell.HasOpened = false;
                cell.IsFlagged = false;
                cell.NeighboringMineCount = 0;
                genCells[rowPos, columnPos] = cell;
            }
        }
        return genCells;
    }
    private List<T> GetNeighbors(int rowPos, int columnPos)
    {
        var neighbors = new List<T>();
        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
            {
                var neighborColumnPos = columnPos + columnOffset;
                var neighborRowPos = rowPos + rowOffset;

                if ((neighborColumnPos == columnPos &&
                neighborRowPos == rowPos) ||
                neighborRowPos >= this.RowCount ||
                neighborColumnPos >= this.ColumnCount ||
                neighborRowPos < 0 ||
                neighborColumnPos < 0)
                    continue;
                else
                {
                    neighbors.Add(this.GetCell(neighborRowPos, neighborColumnPos));
                }
            }
        }
        return neighbors;
    }

}
