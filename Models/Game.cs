using System;
using Avalonia.Threading;
using System.Collections.Generic;

namespace minesweeper.Models;

public class Game<T> where T : ICell<T>, new()
{
    private int GridSize { get; set; }
    public int NonOpenedCellCount { get; private set; }
    public int FlaggedCellCount { get; private set; }
    public int FlaggedMineCount { get; private set; }
    public int MineCount { get; private set; }

    public string username;
    public Scoreboard scoreboard;
    private DispatcherTimer? timer;
    public int GameSeconds;
    public int Score;
    public event EventHandler? SecondPassed;
    public event EventHandler? GameOver;
    public event EventHandler? OpenedCell;
    public int OpenCount;
    public bool IsOver { get; private set; }
    public bool IsWin { get; private set; }

    private T[,] cells;

    public T GetCell(int rowPos, int columnPos)
    {
        return this.cells[rowPos, columnPos];
    }

    public void OpenCell(int rowPos, int columnPos)
    {
        if (
            rowPos >= this.GridSize ||
            columnPos >= this.GridSize ||
            rowPos < 0 ||
            columnPos < 0
        ) return;
        var cell = this.GetCell(rowPos, columnPos);
        this.OpenCell(cell);
    }

    public void OpenCell(T cell)
    {
        if (this.IsOver) return;
        if (cell.IsFlagged) this.ToggleFlag(cell);
        this.OpenCount++;
        this.OpenedCell?.Invoke(this, EventArgs.Empty);
        if (cell.HasMine)
        {
            cell.HasOpened = true;
            this.EndGame(false);
            return;
        }
        var openCells = new List<T>();
        this.openCellRecursive(cell, openCells);
        this.NonOpenedCellCount -= openCells.Count;
        if (this.NonOpenedCellCount <= 0)
        {
            this.EndGame(true);
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


    public void ToggleFlag(T cell)
    {
        if (this.IsOver) return;
        if (cell.IsFlagged)
        {
            cell.IsFlagged = false;
            if (cell.HasMine) this.FlaggedMineCount--;
        }
        else if (!cell.HasOpened)
        {
            cell.IsFlagged = true;
            if (cell.HasMine) this.FlaggedMineCount++;
        }
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

    public Game(string username, Scoreboard scoreboard, int gridSize, int mineCount)
    {
        this.GridSize = gridSize;
        this.MineCount = mineCount;
        this.username = username;
        this.scoreboard = scoreboard;
        this.OpenCount = 0;

        this.cells = this.GenerateCells();
        this.NonOpenedCellCount = gridSize * gridSize - mineCount;
        this.FlaggedCellCount = 0;

        this.GenerateMines(mineCount);
        this.CountNeighborMines();
        this.GameSeconds = 0;

    }

    public void StartTimer()
    {
        this.timer = new DispatcherTimer();
        this.timer.Interval = TimeSpan.FromSeconds(1);
        this.timer.Tick += (sender, e) => this.updateTimer();
        this.timer.Start();
    }

    private void EndGame(bool win)
    {
        this.timer?.Stop();
        foreach (var cell in this.cells)
        {
            if (win && !cell.IsFlagged && cell.HasMine)
            {
                ToggleFlag(cell);
            }
            else if (cell.HasMine) cell.HasOpened = true;


        }
        if (this.GameSeconds == 0) this.GameSeconds = 1;
        this.Score = (int)(((float)this.FlaggedMineCount / (float)this.GameSeconds) * 1000.0);
        this.IsWin = win;
        this.IsOver = true;
        this.GameOver?.Invoke(this, EventArgs.Empty);
        this.scoreboard.AddScore(this.username, this.Score);
    }

    private void updateTimer()
    {
        this.GameSeconds++;
        this.SecondPassed?.Invoke(this, EventArgs.Empty);
    }

    private void GenerateMines(int count)
    {
        var random = new Random();
        for (int i = 0; i < count && i < this.GridSize * this.GridSize; i++)
        {
            int rowPos;
            int columnPos;
            do
            {
                columnPos = random.Next(this.GridSize);
                rowPos = random.Next(this.GridSize);

            } while (this.cells[columnPos, rowPos].HasMine);
            this.cells[columnPos, rowPos].HasMine = true;
        }
    }
    private void CountNeighborMines()
    {
        for (int rowPos = 0; rowPos < this.GridSize; rowPos++)
        {
            for (int columnPos = 0; columnPos < this.GridSize; columnPos++)
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
        var genCells = new T[this.GridSize, this.GridSize];
        for (int rowPos = 0; rowPos < this.GridSize; rowPos++)
        {
            for (int columnPos = 0; columnPos < this.GridSize; columnPos++)
            {
                var cell = new T();
                cell.RowPos = rowPos;
                cell.ColumnPos = columnPos;
                cell.HasMine = false;
                cell.HasOpened = false;
                cell.IsFlagged = false;
                cell.NeighboringMineCount = 0;
                cell.ToggleFlagEvent += (sender, e) => this.ToggleFlag(cell);
                cell.OpenCellEvent += (sender, e) => this.OpenCell(cell);
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
                neighborRowPos >= this.GridSize ||
                neighborColumnPos >= this.GridSize ||
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
