using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProLab.Data.Entities.Routes
{
    public class OptimizedRoute
    {
        public List<int> VisitOrder { get; set; } = new();
        public List<RouteSegment> Segments { get; set; } = new();
        public double TotalDistance { get; set; }
        public double TotalDuration { get; set; }
        public List<Coordinate> FullGeometry { get; set; } = new();
    }
}
