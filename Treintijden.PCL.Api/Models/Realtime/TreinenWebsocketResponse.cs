using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treintijden.PCL.Api.Models.Realtime
{
  public class TreinenWebsocketResponse
  {
    public List<Trein> Treinen { get; set; }

  }

  public class Trein
  {
    public int TreinNummer { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public double Snelheid { get; set; }
    public double Richting { get; set; }
    public string Type { get; set; }


    public override string ToString()
    {
      return "Trein{treinNummer=" + this.TreinNummer + ", lat=" + this.Lat + ", lng=" + this.Lng + ", snelheid=" + this.Snelheid + ", richting=" + this.Richting + '}';
    }

    public bool IsSprinter()
    {
      return "SPR" == (this.Type.ToUpperInvariant());
    }

    public bool IsIntercity()
    {
      return "IC" == (this.Type.ToUpperInvariant());
    }

    public bool IsStilstaand()
    {
      return this.Snelheid < 5.0f;
    }

    public bool IsBewegend()
    {
      return this.Snelheid >= 5.0f;
    }

  }
}
