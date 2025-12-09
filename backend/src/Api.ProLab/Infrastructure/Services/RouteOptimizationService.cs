using ProLab.Data.Entities.Routes;
using ProLab.Data.Enums;

namespace ProLab.Api.Infrastructure.Services
{
    public class RouteOptimizationService
    {
        private readonly MapboxService _mapboxService;
        private readonly RouteSelectionService _routeSelectionService;

        public RouteOptimizationService(
            MapboxService mapboxService,
            RouteSelectionService routeSelectionService)
        {
            _mapboxService = mapboxService;
            _routeSelectionService = routeSelectionService;
        }

        public async Task<OptimizedRoute> OptimizeWithNearestNeighborAsync(
            List<Coordinate> coordinates,
            int startIndex = 0,
            CancellationToken cancellationToken = default)
        {
            var distanceMatrix = await _mapboxService.GetDistanceMatrixAsync(
                coordinates,
                cancellationToken);

            var (visitOrder, totalDuration) = NearestNeighborAlgorithm(distanceMatrix, startIndex);

            var orderedCoordinates = visitOrder.Select(i => coordinates[i]).ToList();
            var routeGeometry = await _mapboxService.GetRouteGeometryAsync(
                orderedCoordinates,
                cancellationToken);

            return new OptimizedRoute
            {
                VisitOrder = visitOrder,
                TotalDuration = routeGeometry.Duration,
                TotalDistance = routeGeometry.Distance,
                FullGeometry = routeGeometry.Geometry,
                Segments = CreateSegments(visitOrder, distanceMatrix, coordinates)
            };
        }

        public async Task<OptimizedRoute> OptimizeWithAlternativesAsync(
            List<Coordinate> coordinates,
            int startIndex = 0,
            SelectionStrategy strategy = SelectionStrategy.Fastest,
            CancellationToken cancellationToken = default)
        {
            var n = coordinates.Count;
            var visited = new bool[n];
            var visitOrder = new List<int> { startIndex };
            var segments = new List<RouteSegment>();
            var fullGeometry = new List<Coordinate>();

            visited[startIndex] = true;
            var current = startIndex;
            var totalDistance = 0.0;
            var totalDuration = 0.0;

            for (var step = 1; step < n; step++)
            {
                AlternativeRoute? bestRoute = null;
                var bestNext = -1;

                for (var j = 0; j < n; j++)
                {
                    if (visited[j]) continue;

                    var alternatives = await _mapboxService.GetAlternativeRoutesAsync(
                        coordinates[current],
                        coordinates[j],
                        cancellationToken);

                    if (alternatives.Count == 0) continue;

                    var selectedRoute = SelectRouteByStrategy(alternatives, strategy);

                    if (bestRoute == null || selectedRoute.Duration < bestRoute.Duration)
                    {
                        bestRoute = selectedRoute;
                        bestNext = j;
                    }
                }

                if (bestRoute == null || bestNext == -1)
                    throw new InvalidOperationException("Nevar atrast nakamo punktu");

                segments.Add(new RouteSegment
                {
                    FromIndex = current,
                    ToIndex = bestNext,
                    Distance = bestRoute.Distance,
                    Duration = bestRoute.Duration,
                    Weight = bestRoute.Weight,
                    Summary = bestRoute.Summary,
                    Geometry = bestRoute.Geometry
                });

                fullGeometry.AddRange(bestRoute.Geometry);

                visitOrder.Add(bestNext);
                visited[bestNext] = true;
                totalDistance += bestRoute.Distance;
                totalDuration += bestRoute.Duration;
                current = bestNext;
            }

            return new OptimizedRoute
            {
                VisitOrder = visitOrder,
                Segments = segments,
                TotalDistance = totalDistance,
                TotalDuration = totalDuration,
                FullGeometry = fullGeometry
            };
        }


        private (List<int> route, double totalDistance) NearestNeighborAlgorithm(
            double[,] distanceMatrix,
            int startIndex)
        {
            var n = distanceMatrix.GetLength(0);
            var visited = new bool[n];
            var route = new List<int> { startIndex };

            visited[startIndex] = true;
            var current = startIndex;
            var totalDistance = 0.0;

            for (var step = 1; step < n; step++)
            {
                var nearestIndex = -1;
                var minDistance = double.MaxValue;

                for (var j = 0; j < n; j++)
                {
                    if (visited[j]) continue;

                    var distance = distanceMatrix[current, j];
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestIndex = j;
                    }
                }

                if (nearestIndex == -1)
                    throw new InvalidOperationException("Nevar atrast nakamo punktu");

                route.Add(nearestIndex);
                visited[nearestIndex] = true;
                totalDistance += minDistance;
                current = nearestIndex;
            }

            return (route, totalDistance);
        }

        //todo: Maybe to use this, if we have enough time and willingness for complete realisation
        public (List<int> improvedRoute, double improvedDistance) TwoOptImprovement(
            List<int> route,
            double[,] distanceMatrix,
            int maxIterations = 100)
        {
            var improved = true;
            var currentRoute = new List<int>(route);
            var currentDistance = CalculateTotalDistance(currentRoute, distanceMatrix);
            var iteration = 0;

            while (improved && iteration < maxIterations)
            {
                improved = false;
                iteration++;

                for (var i = 1; i < currentRoute.Count - 1; i++)
                {
                    for (var j = i + 1; j < currentRoute.Count; j++)
                    {
                        var newRoute = TwoOptSwap(currentRoute, i, j);
                        var newDistance = CalculateTotalDistance(newRoute, distanceMatrix);

                        if (newDistance < currentDistance)
                        {
                            currentRoute = newRoute;
                            currentDistance = newDistance;
                            improved = true;
                        }
                    }
                }
            }

            return (currentRoute, currentDistance);
        }

        private List<int> TwoOptSwap(List<int> route, int i, int j)
        {
            var newRoute = new List<int>();

            newRoute.AddRange(route.Take(i));

            newRoute.AddRange(route.Skip(i).Take(j - i + 1).Reverse());

            newRoute.AddRange(route.Skip(j + 1));

            return newRoute;
        }

        private double CalculateTotalDistance(List<int> route, double[,] distanceMatrix)
        {
            var total = 0.0;
            for (var i = 0; i < route.Count - 1; i++)
            {
                total += distanceMatrix[route[i], route[i + 1]];
            }
            return total;
        }

        private AlternativeRoute SelectRouteByStrategy(
            List<AlternativeRoute> alternatives,
            SelectionStrategy strategy)
        {
            return strategy switch
            {
                SelectionStrategy.Fastest => _routeSelectionService.SelectFastest(alternatives),
                SelectionStrategy.Shortest => _routeSelectionService.SelectShortest(alternatives),
                SelectionStrategy.Balanced => _routeSelectionService.SelectBalanced(alternatives),
                SelectionStrategy.LightestWeight => _routeSelectionService.SelectLightestWeight(alternatives),
                SelectionStrategy.TimeOfDay => _routeSelectionService.SelectByTimeOfDay(alternatives),
                _ => _routeSelectionService.SelectFastest(alternatives)
            };
        }

        private List<RouteSegment> CreateSegments(
            List<int> visitOrder,
            double[,] distanceMatrix,
            List<Coordinate> coordinates)
        {
            var segments = new List<RouteSegment>();

            for (var i = 0; i < visitOrder.Count - 1; i++)
            {
                var from = visitOrder[i];
                var to = visitOrder[i + 1];

                segments.Add(new RouteSegment
                {
                    FromIndex = from,
                    ToIndex = to,
                    Duration = distanceMatrix[from, to],
                    Distance = 0,
                });
            }

            return segments;
        }
    }

    
}
