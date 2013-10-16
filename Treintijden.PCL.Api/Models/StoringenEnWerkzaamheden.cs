using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treintijden.PCL.Api.Models
{
    public class StoringenEnWerkzaamheden
    {
        public List<Storing> Storingen { get; set; }
        public List<Werkzaamheden> Werkzaamheden { get; set; }
    }
}
