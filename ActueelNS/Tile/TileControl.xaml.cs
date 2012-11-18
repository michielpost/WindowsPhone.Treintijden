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
    public partial class TileControl : UserControl
    {

        public TileControl(string name, string code)
        {
            InitializeComponent();

            if (code.Length > 0)
            {
                code = code.Substring(0, 1).ToUpper() + code.Substring(1);
            }

            if(!string.IsNullOrEmpty(code))
                TileTextBlock.Text = code;

            Tile2TextBlock.Text = name;
        }
    }
}
