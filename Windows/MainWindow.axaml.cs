using Avalonia.Controls;
using minesweeper.Controls;

namespace minesweeper.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var field = new FieldControl(30, 30, 10, this);
        MainStackPanel.Children.Add(field);
    }
}
