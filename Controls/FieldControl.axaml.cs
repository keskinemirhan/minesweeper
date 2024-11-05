using minesweeper.Models;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace minesweeper.Controls;

public partial class FieldControl : UserControl
{
    private Game<CellControl> game;
    public FieldControl(int rowCount, int columnCount, int bombCount, Window window)
    {
        InitializeComponent();
        this.game = new Game<CellControl>(rowCount, columnCount, bombCount);
        MainGrid.RowDefinitions.Clear();
        MainGrid.ColumnDefinitions.Clear();
        for (int i = 0; i < rowCount; i++)
        {
            var rowDef = new RowDefinition();
            rowDef.Height = GridLength.Star;
            MainGrid.RowDefinitions.Add(rowDef);
        }

        for (int i = 0; i < columnCount; i++)
        {
            var colDef = new ColumnDefinition();
            colDef.Width = GridLength.Star;
            MainGrid.ColumnDefinitions.Add(colDef);
        }

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
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
        window.Resized += (sender, e) =>
        {
            this.Width = window.Width;
            this.Height = window.Height;
        };
    }


    private void onGameOver()
    {
        ScoreboardButton.IsVisible = true;
        MenuButton.IsVisible = true;
        GameOverIndicator.Text = $"Game Over! You " + (this.game.HasWon ? "Won! " : "Lose! ") + $"With {this.game.Score.ToString()} Points!";
        GameOverIndicator.Foreground = this.game.HasWon ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        RetryButton.IsVisible = true;
        GameOverIndicator.IsVisible = true;


    }
}
