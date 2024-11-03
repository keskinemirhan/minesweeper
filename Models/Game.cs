using System;
using System.Collections.Generic;

namespace minesweeper.Models;

public class Game<T> where T : ICell, new()
{
    public int RowCount { get; private set; }
    public int ColumnCount { get; private set; }
    public int NonOpenedCellCount { get; private set; }
    public int FlaggedCellCount { get; private set; }
    private T[,] cells;

    public T GetCell(int rowPos, int columnPos)
    {
        return this.cells[rowPos, columnPos];
    }
    public List<T> OpenCell(int rowPos, int columnPos)
    {
        if (rowPos >= this.RowCount ||
        columnPos >= this.ColumnCount ||
        rowPos < 0 ||
        columnPos < 0)
            return new List<T>();
        var cell = this.GetCell(rowPos, columnPos);
        if (cell.HasOpened) return new List<T>();
        var openCells = new List<T>
        {
            cell
        };
        cell.HasOpened = true;
        if (cell.NeighboringMineCount == 0)
        {
            foreach (var neighbor in this.GetNeighbors(rowPos, columnPos))
            {
                if (!neighbor.HasOpened)
                {
                    openCells.Add(neighbor);
                    var nOfNeighbor = this.OpenCell(neighbor.RowPos, neighbor.ColumnPos);
                    openCells.AddRange(nOfNeighbor);
                }
            }
        }
        this.NonOpenedCellCount -= openCells.Count;
        return openCells;
    }

    public T FlagCell(int rowPos, int columnPos)
    {
        var cell = this.GetCell(rowPos, columnPos);
        cell.IsFlagged = true;
        return cell;
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
    public T UnFlagCell(int rowPos, int columnPos)
    {
        var cell = this.GetCell(rowPos, columnPos);
        cell.IsFlagged = false;
        return cell;
    }
    public Game(int rowCount, int columnCount, int bombCount)
    {
        this.RowCount = rowCount;
        this.ColumnCount = columnCount;
        this.GenerateCells();
        this.GenerateMines(bombCount);
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
    private void GenerateCells()
    {
        this.cells = new T[this.RowCount, this.ColumnCount];
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
                this.cells[rowPos, columnPos] = cell;
            }
        }
        this.NonOpenedCellCount = this.RowCount * this.ColumnCount;
        this.FlaggedCellCount = 0;
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
