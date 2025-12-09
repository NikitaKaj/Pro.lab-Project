using ProLab.Data.Enums;

namespace ProLab.Api.Requests.Routes
{
    public class OptimizeRouteRequest
    {
        public List<CoordinateDto> Coordinates { get; set; } = new();
        public int StartIndex { get; set; } = 0;
        public OptimizationAlgorithm Algorithm { get; set; } = OptimizationAlgorithm.NearestNeighbor;
        public SelectionStrategy? SelectionStrategy { get; set; }
    }

    public class CoordinateDto
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
