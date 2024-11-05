using Avalonia.Controls;
using System.Collections.ObjectModel;
using minesweeper.Models;

namespace minesweeper.Windows;

public partial class ScoreboardWindow : Window
{
    public ObservableCollection<PlayerScore> PlayerScores { get; }
    public ScoreboardWindow(Scoreboard scoreboard)
    {
        DataContext = this;
        this.PlayerScores = new ObservableCollection<PlayerScore>(scoreboard.PlayerScoreList);
        InitializeComponent();
    }
}
