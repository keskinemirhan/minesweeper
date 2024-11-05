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

    public Game<CellControl> game;
    public FieldControl(string username, Scoreboard scoreboard, int gridSize, int bombCount, Window window)
    {
        InitializeComponent();
        this.game = new Game<CellControl>(username, scoreboard, gridSize, bombCount);
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
                var cell = this.game.GetCell(i, j);
                Grid.SetRow(cell, i);
                Grid.SetColumn(cell, j);
                MainGrid.Children.Add(cell);
            }
        }
        this.game.SecondPassed += (sender, e) => TimeIndicator.Text = "Time: " + this.game.GameSeconds.ToString();
        this.game.SecondPassed += (sender, e) => Console.WriteLine(this.game.GameSeconds);
        this.game.GameOver += (sender, e) =>
        {
            this.onGameOver();
            this.Width = window.Width;
            this.Height = window.Height;
        };
        this.game.StartTimer();
        this.game.OpenedCell += (sender, e) =>
        {
            MoveIndicator.Text = "Moves: " + this.game.OpenCount.ToString();
        };
        this.Width = window.Width;
        this.Height = window.Height;
        window.Resized += (sender, e) =>
        {
            this.Width = window.Width;
            this.Height = window.Height;
        };
    }


    private void onGameOver()
    {
        ScoreboardButton.IsVisible = true;
        GameOverIndicator.Text = $"Game Over! You " + (this.game.HasWon ? "Won! " : "Lose! ") + $"With {this.game.Score.ToString()} Points!";
        GameOverIndicator.Foreground = this.game.HasWon ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
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
