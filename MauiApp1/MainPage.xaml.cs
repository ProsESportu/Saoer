
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<ObservableCollection<Tile>> Tiles { get; set; }
        public bool wasGenerated { get; set; }
        public MainPage()
        {
            InitializeComponent();
            wasGenerated = false;
            Tiles = empty(10, 10);
            gr.ItemsSource = Tiles;

        }
        public void LongPressCommand(object sender)
        {
            //MenuItem button = sender as MenuItem;
            //Tile data = (Tile)button.BindingContext;
            Console.WriteLine("here");
            Button button = sender as Button;
            Tile data = (Tile)button.BindingContext;
            try
            {
                Tiles[data.Coords.x][data.Coords.y].IsFlagged = !Tiles[data.Coords.x][data.Coords.y].IsFlagged;
            }
            catch { }

        }


        public ObservableCollection<ObservableCollection<Tile>> empty(int width, int height)
        {
            var t = new ObservableCollection<ObservableCollection<Tile>>();
            for (int i = 0; i < width; i++)
            {
                var u = new ObservableCollection<Tile>();
                for (int j = 0; j < height; j++)
                {
                    u.Add(new Tile { Value = ".", Coords = new Coords { y = j, x = i } });
                }
                t.Add(u);
            }
            return t;
        }
        public void Rec(Tile tile)
        {
            if (tile.Value != "X" && !tile.IsUncovered)
            {
                tile.IsUncovered = true;
                if (tile.Value == "0")
                {

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            try
                            {

                                Rec(Tiles[tile.Coords.x + i][tile.Coords.y + j]);
                            }
                            catch { }
                        }
                    }
                }
            }
        }


        public ObservableCollection<ObservableCollection<Tile>> genTiles(ObservableCollection<ObservableCollection<Tile>> board, int mineCount, Tile first)
        {
            var rnd = new Random();
            for (int i = 0; i < mineCount; i++)
            {
                int x = rnd.Next(board.Count);
                int y = rnd.Next(board[0].Count);
                var d = board[x][y];
                if (d.Value != "X" && x != first.Coords.x && y != first.Coords.y)
                {
                    board[x][y].Value = "X";
                }
                else
                {
                    i--;
                }
            }
            for (int i = 0; i < board.Count; i++)
            {
                for (int j = 0; j < board[i].Count; j++)
                {
                    int mines = 0;
                    for (int k = -1; k <= 1; k++)
                    {
                        for (int l = -1; l <= 1; l++)
                        {
                            try
                            {
                                if (board[i + k][j + l].Value == "X")
                                {
                                    mines++;
                                }
                            }
                            catch { }
                        }
                    }
                    if (board[i][j].Value != "X")
                    {
                        board[i][j].Value = mines.ToString();
                    }
                }
            }
            wasGenerated = true;
            return board;
        }
        public partial class Tile : INotifyPropertyChanged
        {
            private bool _isUncovered;
            private bool _isFlagged;
            private string _value;

            // Property for Content
            public string Content
            {
                get => IsFlagged ? "🚩" : IsUncovered ? Value : "_";
            }

            public required Coords Coords { get; set; }

            // Property for Value
            public required string Value
            {
                get => _value;
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        OnPropertyChanged(nameof(Value));
                        OnPropertyChanged(nameof(Content)); // Ensure Content is updated when Value changes
                    }
                }
            }

            // Property for IsUncovered
            public bool IsUncovered
            {
                get => _isUncovered;
                set
                {
                    if (_isUncovered != value)
                    {
                        _isUncovered = value;
                        OnPropertyChanged(nameof(IsUncovered));
                        OnPropertyChanged(nameof(Content)); // Ensure Content is updated when IsUncovered changes
                    }
                }
            }

            // Property for IsFlagged
            public bool IsFlagged
            {
                get => _isFlagged;
                set
                {
                    if (_isFlagged != value)
                    {
                        _isFlagged = value;
                        OnPropertyChanged(nameof(IsFlagged));
                        OnPropertyChanged(nameof(Content)); // Ensure Content is updated when IsFlagged changes
                    }
                }
            }
            public event PropertyChangedEventHandler? PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class Coords
        {
            public int x, y;
        }

        private void Button_Click_1(object sender)
        {
            //gr.ItemsSource = null;
            Button button = sender as Button;
            Tile data = (Tile)button.BindingContext;
            if (!wasGenerated)
            {
                Tiles = genTiles(Tiles, int.Parse(mineCount.Text), data);


            }
            Rec(data);
            this.Tiles[data.Coords.x][data.Coords.y].IsUncovered = true;
            if (data.Value == "X")
            {
                DisplayAlert("You lost", "Mine was pressed", "OK");
                //wasGenerated = false;
            }
            if (Tiles.SelectMany(e => e).Where(e => e.Value != "X").All(e => e.IsUncovered))
            {
                DisplayAlert("You've won", "Congrats", "OK");
                //wasGenerated = false;
            }

            //gr.ItemsSource = this.Tiles;
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            Tiles = empty(int.Parse(xWidth.Text), int.Parse(yHeight.Text));
            //gr.ItemsSource = null;
            gr.ItemsSource = Tiles;
            wasGenerated = false;
        }
        private DateTime startTime;
        private DateTime endTime;
        private void Button_Pressed(object sender, EventArgs e)
        {
            this.startTime = DateTime.Now;
        }

        private void Button_Released(object sender, EventArgs e)
        {
            this.endTime = DateTime.Now;
            if ((endTime - startTime).TotalMilliseconds < 500)
            {
                Button_Click_1(sender);
            }
            else
            {
                LongPressCommand(sender);
            }
        }
    }
}

