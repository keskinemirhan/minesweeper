using Avalonia.Controls;
using minesweeper.Controls;
using minesweeper.Models;

namespace minesweeper.Windows;

public partial class MainWindow : Window
{
    public Scoreboard scoreboard;
    public MainWindow()
    {
        InitializeComponent();
        var startForm = new StartForm();
        this.scoreboard = new Scoreboard();
        startForm.SubmitEvent += (sender, e) =>
        {
            this.DisplayGame(startForm.Username, startForm.GridSize, startForm.MineCount);
        };
        MainStackPanel.Children.Add(startForm);
    }

    public void DisplayGame(string username, int gridSize, int mineCount)
    {
        var field = new FieldControl(username, scoreboard, gridSize, mineCount, this);
        MainStackPanel.Children.Clear();
        MainStackPanel.Children.Add(field);
        field.GoToMenu += (sender, e) =>
        {
            var startForm = new StartForm();
            startForm.SubmitEvent += (sender, e) =>
            {
                this.DisplayGame(startForm.Username, startForm.GridSize, startForm.MineCount);
            };
            MainStackPanel.Children.Clear();
            MainStackPanel.Children.Add(startForm);
        };
        field.RetryGame += (sender, e) =>
        {
            this.DisplayGame(username, gridSize, mineCount);
        };
        field.OpenScoreboard += (sender, e) =>
        {
            var scoreboardWindow = new ScoreboardWindow(scoreboard);
            scoreboardWindow.Show();
        };
    }
}
