using Avalonia.Controls;
using minesweeper.Controls;

namespace minesweeper.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var startForm = new StartForm();
        startForm.SubmitEvent += (sender, e) =>
        {
            this.DisplayGame(startForm.Username, startForm.GridSize, startForm.MineCount);
        };
        MainStackPanel.Children.Add(startForm);
    }

    public void DisplayGame(string username, int gridSize, int mineCount)
    {
        var field = new FieldControl(gridSize, gridSize, mineCount, this);
        System.Console.WriteLine(gridSize);
        MainStackPanel.Children.Clear();
        MainStackPanel.Children.Add(field);
    }
}
