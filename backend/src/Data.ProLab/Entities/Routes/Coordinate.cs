using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProLab.Data.Entities.Routes
{
    public record Coordinate(double Longitude, double Latitude)
    {
        public override string ToString() => $"{Longitude},{Latitude}";
    }
}
