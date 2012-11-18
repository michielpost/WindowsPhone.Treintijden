using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ActueelNS.Tile
{
    public partial class TileAdviesControl : UserControl
    {
        public TileAdviesControl(string from, string to, string date)
        {
            InitializeComponent();

            FromText.Text = from;
            ToText.Text = to;

            Tile2TextBlock.Text = date;

        }
    }
}
