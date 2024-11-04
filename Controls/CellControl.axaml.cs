using Avalonia.Controls;
using minesweeper.Models;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using System;

namespace minesweeper.Controls;


public partial class CellControl : UserControl, ICell<CellControl>
{
    public event EventHandler? OpenCellEvent;
    public event EventHandler? ToggleFlagEvent;
    private bool _hasOpened;
    public bool HasMine { get; set; }
    public bool HasOpened
    {
        get
        {
            return this._hasOpened;
        }
        set
        {
            this._hasOpened = value;
            if (value)
            {
                CellImg.IsVisible = false;
                string imageUrl = "";
                if (this.HasMine)
                {
                    imageUrl = "avares://minesweeper/Assets/mine.png";
                }
                else
                {
                    switch (this.NeighboringMineCount)
                    {
                        case 1:
                            imageUrl = "avares://minesweeper/Assets/one.png";
                            break;
                        case 2:
                            imageUrl = "avares://minesweeper/Assets/two.png";
                            break;
                        case 3:
                            imageUrl = "avares://minesweeper/Assets/three.png";
                            break;
                        case 4:
                            imageUrl = "avares://minesweeper/Assets/four.png";
                            break;
                        case 5:
                            imageUrl = "avares://minesweeper/Assets/five.png";
                            break;
                        case 6:
                            imageUrl = "avares://minesweeper/Assets/six.png";
                            break;
                        case 7:
                            imageUrl = "avares://minesweeper/Assets/seven.png";
                            break;
                        case 8:
                            imageUrl = "avares://minesweeper/Assets/eight.png";
                            break;
                        default:
                            break;
                    }
                }
                if (!String.Equals(imageUrl, ""))
                {
                    var bitmap = new Bitmap(AssetLoader.Open(new Uri(imageUrl)));
                    CellImg.IsVisible = true;
                    CellImg.Source = bitmap;
                }
                CellButton.Background = new ImmutableSolidColorBrush(Color.FromRgb(230, 230, 230));
                CellButton.IsEnabled = false;
            }

        }
    }
    private bool _isFlagged;
    public bool IsFlagged
    {
        get
        {
            return this._isFlagged;
        }
        set
        {
            if (!value)
            {
                CellImg.IsVisible = false;
            }
            else
            {
                var imageUrl = "avares://minesweeper/Assets/flag.png";
                var bitmap = new Bitmap(AssetLoader.Open(new Uri(imageUrl)));

                CellImg.IsVisible = true;
                CellImg.Source = bitmap;
            }
            this._isFlagged = value;
        }
    }
    public int RowPos { get; set; }
    public int ColumnPos { get; set; }
    public int NeighboringMineCount { get; set; }

    public CellControl()
    {
        InitializeComponent();
        CellButton.PointerPressed += (sender, e) =>
        {
            var pEvent = (PointerPressedEventArgs)e;
            if (pEvent.GetCurrentPoint((Control)CellButton).Properties.IsRightButtonPressed)
                ToggleFlagEvent?.Invoke(this, e);
        };
        CellButton.Click += (sender, e) => OpenCellEvent?.Invoke(this, e);
    }

    public override string ToString()
    {

        return "[ Row: " + this.RowPos +
               ", Column: " + this.ColumnPos +
               ", Opened: " + this.HasOpened +
               ", Has Mine: " + this.HasMine +
               ", Flagged: " + this.IsFlagged +
               ", Neighboring Mine Count: " + this.NeighboringMineCount + " ]";
    }

}
