using ProLab.Api.Requests.Routes;

namespace ProLab.Api.Responses.Routes
{
    public class OptimizeRouteResponse
    {
        public List<int> VisitOrder { get; set; } = new();
        public double TotalDistance { get; set; }
        public double TotalDuration { get; set; }
        public List<CoordinateDto> FullGeometry { get; set; } = new();
        public List<RouteSegmentDto>? Segments { get; set; }
    }

    public class RouteSegmentDto
    {
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
        public string Summary { get; set; } = string.Empty;
    }
}
