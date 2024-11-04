using minesweeper.Models;
using Avalonia.Controls;
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
        this.game.StartTimer();
        window.Resized += (sender, e) =>
        {
            this.Width = window.Width;
            this.Height = window.Height;
        };
    }
}
