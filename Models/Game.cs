using System;
using System.Collections.Generic;


namespace minesweeper.Models;

public class Game
{
    public int RowCount { get; private set; }
    public int ColumnCount { get; private set; }
    public int NonOpenedCellCount { get; private set; }
    public int FlaggedCellCount { get; private set; }
    private Cell[,] cells;

    public Cell GetCell(int rowPos, int columnPos)
    {
        return this.cells[rowPos, columnPos];
    }
    public List<Cell> OpenCoordinate(int rowPos, int columnPos)
    {
        if (rowPos > this.RowCount ||
        columnPos > this.ColumnCount ||
        rowPos < 0 ||
        columnPos < 0)
            return new List<Cell>();
        var cell = this.GetCell(rowPos, columnPos);
        if (cell.HasOpened) return new List<Cell>();
        var openCells = new List<Cell>
        {
            cell
        };
        if (cell.NeighboringMineCount == 0)
        {
            foreach (var neighbor in this.GetNeighbors(rowPos, columnPos))
            {
                openCells.Add(neighbor);
                var nOfNeighbor = this.OpenCoordinate(neighbor.RowPos, neighbor.ColumnPos);
                openCells.AddRange(nOfNeighbor);
            }
        }
        this.NonOpenedCellCount -= openCells.Count;
        return openCells;
    }

    public Cell FlagCell(int rowPos, int columnPos)
    {
        var cell = this.GetCell(rowPos, columnPos);
        cell.IsFlagged = true;
        return cell;
    }
    public List<Cell> GetMineCells()
    {
        var mineCells = new List<Cell>();
        foreach (var cell in this.cells)
        {
            if (cell.HasMine) mineCells.Add(cell);
        }
        return mineCells;
    }
    public Cell UnFlagCell(int rowPos, int columnPos)
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
        this.cells = new Cell[this.RowCount, this.ColumnCount];
        for (int rowPos = 0; rowPos < this.RowCount; rowPos++)
        {
            for (int columnPos = 0; columnPos < this.ColumnCount; columnPos++)
            {
                this.cells[rowPos, columnPos] = new Cell(rowPos, columnPos);
            }
        }
        this.NonOpenedCellCount = this.RowCount * this.ColumnCount;
        this.FlaggedCellCount = 0;
    }
    private List<Cell> GetNeighbors(int rowPos, int columnPos)
    {
        var neighbors = new List<Cell>();
        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int columnOffset = -1; rowOffset <= 1; rowOffset++)
            {
                var neighborColumnPos = columnPos + columnOffset;
                var neighborRowPos = rowPos + rowOffset;

                if ((neighborColumnPos == columnPos &&
                neighborColumnPos == rowPos) ||
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