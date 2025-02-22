using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Domain.Entities.Stream
{
    public class Angel
    {
        public double V { get; set; }
        public double H { get; set; }
    }

    public class Position
    {
        public double Lat { get; set; }
        public double Long { get; set; }
    }
}