using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treintijden.PCL.Api.Models
{
	public class TreinInfo
	{
		public List<Materieeldelen> Materieeldelen { get; set; }
		public int Lengte { get; set; }

	}

	public class Materieeldelen
	{
		public string Type { get; set; }
		public List<string> Faciliteiten { get; set; }
	}

}
