﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActueelNS.Services.Models
{

    public class RitInfoStop
    {
        public string Code { get; set; }
        public string Station { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Departure { get; set; }
        public int? Prognose { get; set; }
    }


}
