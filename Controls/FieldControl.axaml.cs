using minesweeper.Models;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace minesweeper.Controls;

public partial class FieldControl : UserControl
{
    public event EventHandler? OpenScoreboard;
    public event EventHandler? RetryGame;
    public event EventHandler? GoToMenu;

    public Game<CellControl> Game;
    public FieldControl(string username, Scoreboard scoreboard, int gridSize, int bombCount, Window window)
    {
        InitializeComponent();
        this.Game = new Game<CellControl>(username, scoreboard, gridSize, bombCount);
        MainGrid.RowDefinitions.Clear();
        MainGrid.ColumnDefinitions.Clear();
        for (int i = 0; i < gridSize; i++)
        {
            var rowDef = new RowDefinition();
            rowDef.Height = GridLength.Star;
            MainGrid.RowDefinitions.Add(rowDef);
        }

        for (int i = 0; i < gridSize; i++)
        {
            var colDef = new ColumnDefinition();
            colDef.Width = GridLength.Star;
            MainGrid.ColumnDefinitions.Add(colDef);
        }

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                var cell = this.Game.GetCell(i, j);
                Grid.SetRow(cell, i);
                Grid.SetColumn(cell, j);
                MainGrid.Children.Add(cell);
            }
        }
        this.Game.SecondPassed += (sender, e) => TimeIndicator.Text = "Time: " + this.Game.GameSeconds.ToString();
        this.Game.GameOver += (sender, e) =>
        {
            this.OnGameOver();
            this.Width = window.Width;
            this.Height = window.Height;
        };
        this.Game.StartTimer();
        this.Game.OpenedCell += (sender, e) =>
        {
            MoveIndicator.Text = "Moves: " + this.Game.OpenCount.ToString();
        };
        this.Width = window.Width;
        this.Height = window.Height;
        window.Resized += (sender, e) =>
        {
            this.Width = window.Width;
            this.Height = window.Height;
        };
    }


    public void OnGameOver()
    {
        ScoreboardButton.IsVisible = true;
        GameOverIndicator.Text = $"Game Over! You " + (this.Game.IsWin ? "Won! " : "Lose! ") + $"With {this.Game.Score.ToString()} Points!";
        GameOverIndicator.Foreground = this.Game.IsWin ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        RetryButton.Click += (sender, e) =>
        {
            this.RetryGame?.Invoke(this, EventArgs.Empty);
        };
        ScoreboardButton.Click += (sender, e) =>
        {
            this.OpenScoreboard?.Invoke(this, EventArgs.Empty);
        };
        MenuButton.Click += (sender, e) =>
        {
            this.GoToMenu?.Invoke(this, EventArgs.Empty);
        };
        MenuButton.IsVisible = true;
        RetryButton.IsVisible = true;
        GameOverIndicator.IsVisible = true;

    }
}
