using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProLab.Data.Entities.Routes
{
    public class AlternativeRoute
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
        public double Weight { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<Coordinate> Geometry { get; set; } = new();
    }
}
