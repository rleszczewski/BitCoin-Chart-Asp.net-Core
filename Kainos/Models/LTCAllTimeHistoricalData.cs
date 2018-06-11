using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kainos.Models
{
    public class LTCAllTimeHistoricalData
    {
        public DateTime? time { get; set; }
        public double? average { get; set; }
        public double? volume { get; set; }
        public double? low { get; set; }
        public double? open { get; set; }
    }
}
