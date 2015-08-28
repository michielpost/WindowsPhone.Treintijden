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
using Treintijden.PCL.Api.Models;

namespace ActueelNS.Tile
{
  public partial class TileAdviesControl : UserControl
  {
    public TileAdviesControl(ReisMogelijkheid mogelijkheid, string from, string to, string date)
    {
      InitializeComponent();

      VertrekSpoor.Text = string.Empty;
      AankomstSpoor.Text = string.Empty;

      FromText.Text = from;
      ToText.Text = to;

      Tile2TextBlock.Text = date;

      if (mogelijkheid != null && mogelijkheid.ReisDelen != null)
      {
        var vertrekspoor = mogelijkheid.ReisDelen.FirstOrDefault()?.ReisStops?.FirstOrDefault()?.Vertrekspoor;
        var aankomstspoor = mogelijkheid.ReisDelen.LastOrDefault()?.ReisStops?.LastOrDefault()?.Vertrekspoor;

        VertrekSpoor.Text = vertrekspoor;
        AankomstSpoor.Text = aankomstspoor;
      }

    }
  }
}
