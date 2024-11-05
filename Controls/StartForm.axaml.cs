using Avalonia.Controls;
using System;

namespace minesweeper.Controls;

public partial class StartForm : UserControl
{
    public event EventHandler? SubmitEvent;
    public string Username = "";
    public int GridSize = 0;
    public int MineCount = 0;

    public StartForm()
    {
        InitializeComponent();
        SubmitButton.Click += (sender, e) => this.OnSubmit();
    }

    public void OnSubmit()
    {
        var errors = "";
        var username = UserNameControl.Text?.Trim();
        if (username == null || username == "")
        {
            errors += "Please enter an username!\n";
        }

        int gridSizeInt = 0;
        var gridSize = GridSizeControl.Text;
        var isGridSizeNum = int.TryParse(gridSize, out gridSizeInt);
        if (!isGridSizeNum)
        {
            errors += "Please enter an integer as grid size!\n";
        }

        else if (gridSizeInt > 30)
        {
            errors += "Please enter a number less than or equal to 30 as grid size!\n";
        }

        int mineCountInt = 0;
        var mineCount = MineCountControl.Text;
        var isMineCountNum = int.TryParse(mineCount, out mineCountInt);
        if (!isMineCountNum)
        {
            errors += "Please enter an integer as number of mines!\n";
        }
        else if (mineCountInt < 10)
        {
            errors += "Please enter a number greater than 10 as number of mines\n";
        }

        if (errors == "")
        {
            this.MineCount = mineCountInt;
            this.GridSize = gridSizeInt;
            this.Username = username;
            this.SubmitEvent?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            ErrorControl.Text = errors;
        }



    }

}
