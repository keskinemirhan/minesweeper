using System;
using Avalonia.Threading;
using System.Collections.Generic;

namespace minesweeper.Models;

public class Game<T> where T : ICell<T>, new()
{
    private int gridSize;
    private int nonOpenedCellCount;
    private int flaggedMineCount;
    private T[,] cells;
    private DispatcherTimer? timer;

    public Scoreboard Scoreboard;

    public string Username { get; private set; }
    public int GameSeconds { get; private set; }
    public int Score { get; private set; }
    public int OpenCount { get; private set; }
    public bool IsOver { get; private set; }
    public bool IsWin { get; private set; }

    public event EventHandler? SecondPassed;
    public event EventHandler? GameOver;
    public event EventHandler? OpenedCell;


    public T GetCell(int rowPos, int columnPos)
    {
        return this.cells[rowPos, columnPos];
    }

    public void StartTimer()
    {
        this.timer = new DispatcherTimer();
        this.timer.Interval = TimeSpan.FromSeconds(1);
        this.timer.Tick += (sender, e) => this.updateTimer();
        this.timer.Start();
    }

    public Game(string username, Scoreboard scoreboard, int gridSize, int mineCount)
    {
        this.gridSize = gridSize;
        this.Username = username;
        this.Scoreboard = scoreboard;
        this.OpenCount = 0;

        this.cells = this.generateCells();
        this.nonOpenedCellCount = gridSize * gridSize - mineCount;

        this.generateMines(mineCount);
        this.countNeighborMines();
        this.GameSeconds = 0;
    }

    private void openCell(T cell)
    {
        if (this.IsOver) return;
        if (cell.IsFlagged) this.toggleFlag(cell);
        this.OpenCount++;
        this.OpenedCell?.Invoke(this, EventArgs.Empty);
        if (cell.HasMine)
        {
            cell.HasOpened = true;
            this.endGame(false);
            return;
        }
        var openCells = new List<T>();
        this.openCellRecursive(cell, openCells);
        this.nonOpenedCellCount -= openCells.Count;
        if (this.nonOpenedCellCount <= 0)
        {
            this.endGame(true);
        }
    }

    private void openCellRecursive(T cell, List<T> openCells)
    {
        if (cell.HasOpened) return;
        cell.HasOpened = true;
        openCells.Add(cell);
        if (cell.NeighboringMineCount == 0)
        {
            foreach (var neighbor in this.getNeighbors(cell.RowPos, cell.ColumnPos))
            {
                if (!neighbor.HasOpened)
                {
                    this.openCellRecursive(neighbor, openCells);
                }
            }
        }
    }

    private void toggleFlag(T cell)
    {
        if (this.IsOver) return;
        if (cell.IsFlagged)
        {
            cell.IsFlagged = false;
            if (cell.HasMine) this.flaggedMineCount--;
        }
        else if (!cell.HasOpened)
        {
            cell.IsFlagged = true;
            if (cell.HasMine) this.flaggedMineCount++;
        }
    }

    private void endGame(bool win)
    {
        this.timer?.Stop();
        foreach (var cell in this.cells)
        {
            if (win && !cell.IsFlagged && cell.HasMine)
            {
                toggleFlag(cell);
            }
            else if (cell.HasMine) cell.HasOpened = true;


        }
        if (this.GameSeconds == 0) this.GameSeconds = 1;
        this.Score = (int)(((float)this.flaggedMineCount / (float)this.GameSeconds) * 1000.0);
        this.IsWin = win;
        this.IsOver = true;
        this.GameOver?.Invoke(this, EventArgs.Empty);
        this.Scoreboard.AddScore(this.Username, this.Score);
    }

    private void updateTimer()
    {
        this.GameSeconds++;
        this.SecondPassed?.Invoke(this, EventArgs.Empty);
    }

    private void generateMines(int count)
    {
        var random = new Random();
        for (int i = 0; i < count && i < this.gridSize * this.gridSize; i++)
        {
            int rowPos;
            int columnPos;
            do
            {
                columnPos = random.Next(this.gridSize);
                rowPos = random.Next(this.gridSize);

            } while (this.cells[columnPos, rowPos].HasMine);
            this.cells[columnPos, rowPos].HasMine = true;
        }
    }

    private void countNeighborMines()
    {
        for (int rowPos = 0; rowPos < this.gridSize; rowPos++)
        {
            for (int columnPos = 0; columnPos < this.gridSize; columnPos++)
            {
                var cell = this.cells[rowPos, columnPos];
                var neighbors = this.getNeighbors(rowPos, columnPos);
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.HasMine) cell.NeighboringMineCount++;
                }
            }
        }
    }

    private T[,] generateCells()
    {
        var genCells = new T[this.gridSize, this.gridSize];
        for (int rowPos = 0; rowPos < this.gridSize; rowPos++)
        {
            for (int columnPos = 0; columnPos < this.gridSize; columnPos++)
            {
                var cell = new T();
                cell.RowPos = rowPos;
                cell.ColumnPos = columnPos;
                cell.HasMine = false;
                cell.HasOpened = false;
                cell.IsFlagged = false;
                cell.NeighboringMineCount = 0;
                cell.ToggleFlagEvent += (sender, e) => this.toggleFlag(cell);
                cell.OpenCellEvent += (sender, e) => this.openCell(cell);
                genCells[rowPos, columnPos] = cell;
            }
        }
        return genCells;
    }

    private List<T> getNeighbors(int rowPos, int columnPos)
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
                neighborRowPos >= this.gridSize ||
                neighborColumnPos >= this.gridSize ||
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
